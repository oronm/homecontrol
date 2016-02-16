import moment from 'moment';

//export class DateFormatValueConverter {
//    toView(value) {
//        return moment(value).format('M/D/YYYY h:mm:ss a');
//    }
//}
export class DateFormatValueConverter {
    toView(value,format) {
        return moment(value).format(format);
    }
    toView(value) {
        var format = "D/M HH:mm"; 
        return moment(value).format(format);
    }
}