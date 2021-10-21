var inContact = inContact || {};

inContact.Spinner = 
{
    DEFAULT_SPINNER_ID: "spinner",
    _spinner: null,

    spinnerOptions: {
        top: -2,
        left: 0,
        lines: 9,
        length: 4,
        width: 3,
        radius: 6,
        corners: 1,
        color: '#f48B31',
        speed: 1.2,
        trail: 40
    },

    show: function (spinnerId) {

        if (this._spinner === null) {
            this._spinner = new Spinner(this.spinnerOptions);
        }
                
        spinnerId = spinnerId || this.DEFAULT_SPINNER_ID;
        var spinnerElement = document.getElementById(spinnerId);
        this._spinner.spin(spinnerElement);
    },

    hide: function ()
    {
        if (this._spinner != null)
        {
            this._spinner.stop();
        }
    }
};