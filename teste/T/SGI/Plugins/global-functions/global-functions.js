var GlobalFunctions = (function () {
    var publico = {
        secondsToTimeDinamic: function (segundos) {
            var sec_num = parseInt(segundos, 10); // don't forget the second param
            var hours = Math.floor(sec_num / 3600);
            var minutes = Math.floor((sec_num - (hours * 3600)) / 60);
            var seconds = sec_num - (hours * 3600) - (minutes * 60);

            if (hours < 10) { hours = "0" + hours; }
            if (minutes < 10) { minutes = "0" + minutes; }
            if (seconds < 10) { seconds = "0" + seconds; }

            var stringFormat = '';

            if (segundos < 60) {
                stringFormat = seconds + 's';
            }
            else if (segundos < 3600) {
                stringFormat = minutes + 'm ' + seconds + 's';
            }
            else {
                stringFormat = hours + 'h ' + minutes + 'm ' + seconds + 's';
            }
            return stringFormat;
        }
    }
    return publico;
})();