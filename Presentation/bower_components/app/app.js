/**
 * @file 
 * Provides main Backbone view events and models.
 *
 * all te backbone events and models are presence here
 *
 * Author: Viniston Fernando
 */
$(document).ready(function () {

    var AllStation = [],
        formMOdel = {},
        formData = {},
        Notifystatementsdisplaytime = '3';


    /**
     * Backbone view model.
     **/
    var windModel = Backbone.Model.extend({
        idAttribute: 'Id',
        urlRoot: 'api/Common/CreateNewReading',
        defaults: {
            State: '',
            City: '',
            StationCode: '',
            ActualSpeed: 0,
            PredictedSpeed: 0,
            Variance: 0,
            ReadingDate: null,
            Captcha: ''
        }
    });

    /**
     * Backbone view.
     **/

    window.AppView = Backbone.View.extend({

        el: $(".windRegistration"),

        // Main initialization entry point...

        initialize: function () {
            this.hidehistory(); //Hide history container since we are not loading data on page load
            this.initInputForm();
        },

        // event responsible for state change. So we have feed all the cities with respect...
        // to this state.

        stateSelected: function () {
            formData["sel_State"] = $('#select2-state').val();
            $("#select2-city").select2("val", "-Select City-");
            $("#stationcode").val('');
            this.populateCity();
        },

        // event responsible for city change. So we have assign the stationid with respect ...
        // to this state and city. Because there is 1:1 mapping between city and station code.

        populateCity: function () {
            var cities = [];
            cities = $.grep(AllStation, function (rel) {
                return rel.StateName == formData["sel_State"];
            });
            $('#select2-city option').remove();
            $('#select2-city').append($("<option></option>").attr("value", 'Select City-').text('Select City-'));
            if (cities.length > 0)
                for (var i = 0, city = {}; city = cities[i++];) {
                    $('#select2-city').append($("<option></option>").attr("value", city.CityName).text(city.CityName));
                }
        },

        // Auto calculation of the variance (variance = actual - predicted speed) ...

        calculateVariance: function () {
            var predictedSeed = $("#predictedspeed").val(),
                actualSpeed = $("#actualspeed").val(),
                variance = (actualSpeed != '' ? actualSpeed : 0) - (predictedSeed != '' ? predictedSeed : 0);
            $("#variancenumber").val(variance);
            if (variance === 1 || variance == -1) $('#variancenumber').removeClass('purple-textfield').removeClass('red-textfield').addClass('black-textfield');
            else if (variance === 3 || variance === -3) $('#variancenumber').removeClass('red-textfield').removeClass('black-textfield').addClass('purple-textfield');
            else if (variance < -5 || variance > 5) $('#variancenumber').removeClass('black-textfield').removeClass('purple-textfield').addClass('red-textfield');

        },

        // event responsible for auto populate the station code after change state and city...

        populateStationCode: function () {
            formData["sel_State"] = $('#select2-state').val();
            formData["sel_city"] = $('#select2-city').val();
            var station = $.grep(AllStation, function (rel) {
                return rel.StateName == formData["sel_State"] && rel.CityName == formData["sel_city"];
            })[0];
            if (station != null) {
                $("#stationcode").val(station.StationID);
                $("#predictedspeed").val(station.PredictedSpeed);
                this.getPredictedSpeed(station.StationID);
            } else {
                $("#stationcode").val('');
                $("#predictedspeed").val('');
            }
        },

        // NOD.JS validation entry point...

        GetValidationList: function () {
            var str = [
                ['#select2-state', 'presence', 'Please select proper State'],
                ['#select2-city', 'presence', 'Please select proper City'],
                ['#entrydate', 'presence', 'Cannot be empty'],
                ['#actualspeed', 'presence', 'Cannot be empty'],
                ['#WindRegCaptcha', 'presence', 'Cannot be empty']
            ];
            var valcoll = [];
            for (var j = 0; j < str.length; j++) {
                valcoll.push(str[j]);
            }
            $("#Reg").nod(valcoll, {
                'disableSubmitBtn': false,
                'delay': 200,
                'submitBtnSelector': '#btnTemp',
                'silentSubmit': 'true'
            });
        },

        // CAPTCHA - HUMAN VALIDATION...

        generateCaptchaImage: function () {
            var rString = this.uniqueToken(32, '0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ');
            $.ajax({
                url: "api/Authorization/GeneratingCaptchaCookie",
                type: "POST",
                data: {
                    "Location": "Wnreg"
                },
                dataType: "json",
                success: function (e) {
                    $("#WindRegCaptcha").val("");
                    $("#imgCaptcha").attr("src", "GenerateCaptcha.aspx?purpose=Wnreg&time=" + rString);
                },
                error: function (e, o, t) {
                    bootbox.alert(e + "\n" + o + "\n" + t)
                }
            });
            return false;
        },

        uniqueToken: function (length, chars) {
            var result = '';
            for (var i = length; i > 0; --i) result += chars[Math.floor(Math.random() * chars.length)];
            return result;
        },

        //get all the stations from the system
        getStations: function () {

            $.ajax({
                url: "api/Common/GetStations",
                type: "GET",
                dataType: "json",
                async: false,
                success: function (e) {
                    AllStation = e.Response;
                    var States = [];
                    $.each(e.Response, function (index, value) {
                        if ($.inArray(value.StateName, States) === -1) {
                            States.push(value.StateName);
                        }
                    });
                    for (var i = 0, state = {}; state = States[i++];) {
                        $('#select2-state').append($("<option></option>").attr("value", state).text(state));
                    }
                },
                error: function (e, o, t) {
                    bootbox.alert(e + "\n" + o + "\n" + t)
                }
            });
        },


        // Population all the countries ...

        initInputForm: function () {
            $('select').select2();
            $('#entrydate').datepicker({
                format: "MM dd, yyyy",
                language: 'en'
            });
            $('[title!=""]').qtip({
                style: {
                    classes: 'qtip-dark qtip-shadow qtip-rounded"'
                }
            });
            this.GetValidationList();
            this.generateCaptchaImage();
            this.getStations();

        },

        // Submit the Speed entry form with a help of WindModel...

        submitForm: function (e) {
            if (this.validateFormInput()) {
                setTimeout(function () {
                    $("#btnTemp").click();
                }, 200);
                $("#RegMetadata").removeClass('notvalidate');
                if ($("#RegMetadata .error").length > 0) {
                    e.stopImmediatePropagation();
                    e.stopPropagation();
                    return false;
                }
                var cityName = $('#select2-city').val(),
                    stateName = $('#select2-state').val(),
                    actualSpeed = $("#actualspeed").val(),
                    predictedSpeed = $("#predictedspeed").val(),
                    readingDate = $("#entrydate").val(),
                    stationCode = $("#stationcode").val(),
                    variance = $("#variancenumber").val(),
                    captcha = $("#WindRegCaptcha").val();
                var windReading = new windModel({
                    State: stateName,
                    City: cityName,
                    StationCode: stationCode,
                    Variance: variance,
                    ActualSpeed: actualSpeed,
                    PredictedSpeed: predictedSpeed,
                    ReadingDate: readingDate,
                    Captcha: captcha
                });
                windReading.save({}, {
                    success: function (model, response) {
                        if (response.StatusCode == 400) {
                            $('.top-right').notify({
                                message: {
                                    text: "Model is not having all the required data."
                                },
                                type: 'danger',
                                fadeOut: {
                                    enabled: true,
                                    delay: parseInt(Notifystatementsdisplaytime) * 1000
                                }
                            }).show();
                        } else if (response.StatusCode == 403) {
                            $('.top-right').notify({
                                message: {
                                    text: "CAPTCHA Mismatch."
                                },
                                type: 'danger',
                                fadeOut: {
                                    enabled: true,
                                    delay: parseInt(Notifystatementsdisplaytime) * 1000
                                }
                            }).show();
                        } else {
                            $('.top-right').notify({
                                message: {
                                    text: "Wind speed reading successfully created."
                                },
                                type: 'success',
                                fadeOut: {
                                    enabled: true,
                                    delay: parseInt(Notifystatementsdisplaytime) * 1000
                                }
                            }).show();
                            setTimeout(function () {
                                location.reload();
                            }, 500)
                        }

                    },
                    error: function () {
                        $('.top-right').notify({
                            message: {
                                text: "Wind speed reading not created."
                            },
                            type: 'danger',
                            fadeOut: {
                                enabled: true,
                                delay: parseInt(Notifystatementsdisplaytime) * 1000
                            }
                        }).show();
                        setTimeout(function () {
                            location.reload();
                        }, 100)
                    }
                });
                return true;
            } else {
                setTimeout(function () {
                    $("#btnTemp").click();
                }, 200);
                $("#RegMetadata").removeClass('notvalidate');
                if ($("#RegMetadata .error").length > 0) {
                    e.stopImmediatePropagation();
                    e.stopPropagation();
                    return false;
                }

            }
        },

        //Validate the form before submit, eventhough we have nod.js validation in client side 
        //and model validation in server side also

        validateFormInput: function () {
            var valid = true,
                cityName = $('#select2-city').val(),
                stateName = $('#select2-state').val(),
                actualSpeed = $("#actualspeed").val(),
                predictedSpeed = $("#predictedspeed").val(),
                readingDate = $("#entrydate").val(),
                stationCode = $("#stationcode").val(),
                variance = $("#variancenumber").val(),
                captcha = $("#WindRegCaptcha").val();
            //check all the properties have proper values
            if (cityName == '' || stateName == '' || actualSpeed == '' || predictedSpeed == '' || readingDate == '' || stationCode == '' || variance == '' || captcha == '')
                valid = false;
            else
                valid = true;
            return valid;

        },

        //fetch predicted speed from the Station code

        getPredictedSpeed: function (stationcode) {
            if (stationcode != null)
                var jobj = { StationId: stationcode }
            $.ajax({
                url: "api/Common/GetPredictedSpeed",
                type: "POST",
                data: jobj,
                success: function (e) {
                    if (e.Response != null) {
                        $("#predictedspeed").val(e.Response);
                    }
                },
                error: function (e, o, t) {
                    bootbox.alert(e + "\n" + o + "\n" + t)
                }
            });
        },

        // Fetch historical data ...

        historicalData: function () {
            this.sowhistory();
            $.ajax({
                url: "api/Common/GetHistoricalData/10",
                type: "GET",
                success: function (e) {

                    if (e.Response != null) {
                        if (e.Response.length > 0) {
                            var TRs = '';
                            for (var i = 0, TR = {}; TR = e.Response[i++];) {
                                TRs += '<tr>';
                                TRs += '<td>' + TR.State + '</td>';
                                TRs += '<td>' + TR.City + '</td>';
                                TRs += '<td>' + TR.StationCode + '</td>';
                                TRs += '<td>' + TR.PredictedSpeed + '</td>';
                                TRs += '<td>' + TR.ActualSpeed + '</td>';
                                if (TR.Variance === 1 || TR.Variance == -1) TRs += '<td style="color: black; font-weight: bold;">' + TR.Variance + '</td>';
                                else if (TR.Variance === 3 || TR.Variance === -3) TRs += '<td style="color: purple; font-weight: bold;">' + TR.Variance + '</td>';
                                else if (TR.Variance < -5 || TR.Variance > 5) TRs += '<td style="color: red; font-weight: bold;">' + TR.Variance + '</td>';
                                else TRs += '<td>' + TR.Variance + '</td>';
                                TRs += '<td>' + TR.DesiredDate + '</td>';
                                TRs += '</tr>';

                            }
                            $("#tbdyHistory").html(TRs);
                        } else {
                            bootbox.alert("No history present. Please create new speed entries.");
                            $("#historyDiv").hide();
                        }
                    } else {
                        bootbox.alert("No history present. Please create new speed entries.");
                        $("#historyDiv").hide();
                    }
                },
                error: function (e, o, t) {
                    bootbox.alert(e + "\n" + o + "\n" + t)
                }
            });
        },

        // Some set of additional utility events ...

        hidehistory: function () {
            $("#historyDiv").hide();
        },

        expandcollpasehistoryblock: function (e) {
            // Holds the product ID of the clicked element
            var isExpand = $("#expandcollapse").hasClass("icon-minus");
            if (isExpand) {
                $(".historical-wrapper").hide();
                $("#expandcollapse").removeClass("icon-minus infoIcon").addClass("icon-plus infoIcon");
            } else {
                $(".historical-wrapper").show();
                $("#expandcollapse").removeClass("icon-plus infoIcon").addClass("icon-minus infoIcon");
            }
        },

        sowhistory: function () {
            $("#historyDiv").show();
        },

        // Backbone View events ...

        events: {
            "change #select2-state": "stateSelected",
            "change #select2-city": "populateStationCode",
            "change input.speedTrack": "calculateVariance",
            "click #btnCaptchaRfresh": "generateCaptchaImage",
            "click #registerbtn": "submitForm",
            "click #historicalData": "historicalData",
            "click #expandcollapse": "expandcollpasehistoryblock",

        }

    });

    var appview = new AppView();
});