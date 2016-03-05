import moment from 'moment';

export class ElapsedFormatValueConverter {
    toView(value) {
        var elapsed = (Date.now()- Date.parse(value))/1000;
        var result;
        if (elapsed < 60*2)
        {
            result = "Just Now";
        }
        else if (elapsed < 60*5)
        {
            result = "Moments Ago";
        }
        else if (elapsed > 60*60*24*100)
        {
            result = "N/A";
        }
        else
        {
            result = moment(Date.parse(value)).fromNow();
        }

        return result;

    }
}