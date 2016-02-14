using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Development.Core.Interface;
using Development.Dal.Base;

namespace Development.Core
{
    class Transaction : ITransaction
    {
        private string _key = null;
        private bool _isDalTransactionOpen = false;
        private bool _isReadOnly = false;
        private bool _isCommitted = false;
        private bool _isRolledBack = false;
        private bool _isClosed = false;
        private bool _lastTaskWasCommit = false;
        private int _commitCount = 0;

        public Transaction(string key, bool isReadOnly)
        {
            _key = key;
            _isReadOnly = isReadOnly;
        }

        public bool IsOpen
        {
            get { return !_isClosed; }
        }

        public string Key
        {
            get { return _key; }
        }

        public PersistenceManager PersistenceManager
        {
            get
            {
                if (!_isDalTransactionOpen)
                {
                    // Start the transaction
                    PersistenceManager.Instance.BeginTransaction();
                    _isDalTransactionOpen = true;
                }
                return PersistenceManager.Instance;
            }
        }

        public void Commit()
        {
            _lastTaskWasCommit = true;
            _commitCount--;
            if (_commitCount > 0 || _isClosed || _isRolledBack || _isCommitted || _isReadOnly)
            {
                return;
            }

            try
            {
                // Do the actual DAL Commit
                PersistenceManager.Instance.CommitTransaction();
                DevelopmentManagerFactory.TransactionCommited(this);
                _isCommitted = true;
                _isDalTransactionOpen = false;
                Close();
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                try
                {
                    if (!_isCommitted)
                    {
                        Rollback();
                    }
                }
                catch (Exception exc)
                {
                    throw new Exception("Exception when rolling back transaction", exc);
                }
            }
        }

        public void Rollback()
        {
            _lastTaskWasCommit = false;
            if (_isClosed || _isCommitted || _isRolledBack || _isReadOnly)
            {
                return;
            }

            try
            {
                // Do the actual DAL Commit
                PersistenceManager.Instance.RollbackTransaction();
                DevelopmentManagerFactory.TransactionRollbacked(this);
                _isRolledBack = true;
                _isDalTransactionOpen = false;
            }
            finally
            {
                Close();
            }
        }

        public void Close()
        {
            if (_isClosed)
            {
                return;
            }

            DevelopmentManagerFactory.RemoveTransaction(_key);

            try
            {
                // Do the actual DAL Commit
                PersistenceManager.Instance.Close();
                _isDalTransactionOpen = false;
            }
            finally
            {
                _isClosed = true;
            }

            if (!_isReadOnly && !_isCommitted && !_isRolledBack)
            {
                throw new Exception("Transaction not commited or rolled back.");
            }
        }

        internal void IncrementCommitCount()
        {
            _commitCount++;
        }

        public void Dispose()
        {
            if (!_lastTaskWasCommit)
            {
                if (!_isClosed && !_isCommitted)
                {
                    Rollback();
                }
                Close();
            }
            _lastTaskWasCommit = false;
        }
    }
}
