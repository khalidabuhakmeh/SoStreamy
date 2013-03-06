var Streamy = Streamy || {};

Streamy.Colors = [
    { top: 'palette-firm', bottom: 'palette-firm-dark' },
    { top: 'palette-success', bottom: 'palette-success-dark' },
    { top: 'palette-info', bottom: 'palette-info-dark' },
    { top: 'palette-amethyst', bottom: 'palette-wisteria' },
    { top: 'palette-night', bottom: 'palette-night-dark' },
    { top: 'palette-bright', bottom: 'palette-bright-dark' },
    { top: 'palette-carrot', bottom: 'palette-pumpkin' },
    { top: 'palette-alizarin', bottom: 'palette-pomegranate' },
    { top: 'palette-concrete', bottom: 'palette-asbestos' }
];

Streamy.LastColor = null;

Streamy.RandomColor = function () {
    var color = Streamy.LastColor;
    while (color == Streamy.LastColor) {
        color = Streamy.Colors[Math.floor(Math.random() * Streamy.Colors.length)];
    }
    return color;
};

Streamy.ApplicationViewModel = function () {
    var self = this;
    var streamy = $.connection.streamy;

    self.name = ko.observable('');
    self.thought = ko.observable('');
    self.thoughts = ko.observableArray();
    self.messages = ko.observableArray();

    self.any = ko.computed(function () {
        return self.thoughts().length > 0;
    });

    self.empty = ko.computed(function () {
        return !self.any();
    });

    self.create = function () {
        var thought = {
            date: new Date(),
            thought: self.thought(),
            name: self.name()
        };

        streamy.server.submit(thought);

        self.thought('');
    };

    self.add = function (thought) {
        self.thoughts.unshift(new Streamy.ThoughtViewModel(thought));
    };

    self.flash = function (text, type) {
        var message = { text: text, type: type || "alert-success" };
        self.messages.unshift(message);

        setTimeout(function () {
            self.messages.remove(message);
        }, 3000);
    };

    self.show = function (elem, vm) {
        $(elem).hide().show('blind');
    };

    // SignalR methods
    streamy.client.addThought = function (thought) {
        self.add(thought);
    };

    streamy.client.addError = function (message) {
        self.flash(message, 'alert-danger');
    };

    streamy.client.addMessage = function (message) {
        self.flash(message);
    };

    // change to true to see more info from SignalR
    $.connection.hub.logging = true;
    $.connection.hub.start();
};

Streamy.ThoughtViewModel = function (json) {
    var self = this;
    json = ko.toJS(json);

    self.name = json.name || json.Name;
    self.thought = json.thought || json.Thought;
    self.date = json.date || json.Date;
    self.date = new Date(self.date);
    Streamy.LastColor = Streamy.RandomColor();
    self.color = Streamy.LastColor;
};

Streamy.Purge = function() {
    $.post('/thoughts/purge', function(result) {
        console.log(result);
    });
}