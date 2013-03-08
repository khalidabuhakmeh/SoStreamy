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

Streamy.ApplicationViewModel = function (seed) {
    var self = this;
    var streamy = $.connection.streamy;
    seed = seed || { totalThoughts: 0, pageLoaded: new Date() };

    self.name = ko.observable('');
    self.thought = ko.observable('');
    self.thoughts = ko.observableArray();
    self.messages = ko.observableArray();
    self.total = ko.observable(seed.totalThoughts);
    self.loaded = new Date(seed.pageLoaded);
    self.currentPage = 1;

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

    self.more = function () {
        $.post('/thoughts/more',
            { page: self.currentPage, loaded: self.loaded.toISOString() },
            function (result) {
                if (result.ok) {
                    console.log(result);
                    self.currentPage = result.nextPage;
                    for (var t in result.thoughts) {
                        self.thoughts.push(new Streamy.ThoughtViewModel(result.thoughts[t]));
                    }
                }
            });
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

    streamy.client.updateTotal = function (totalItems) {
        self.total(totalItems);
    };

    if (seed && seed.thoughts) {
        for (var i in seed.thoughts) {
            self.thoughts.push(new Streamy.ThoughtViewModel(seed.thoughts[i], true));
        }
    }

    // change to true to see more info from SignalR
    $.connection.hub.logging = false;
    $.connection.hub.start();
};

Streamy.ThoughtViewModel = function (json, html) {
    var self = this;
    json = ko.toJS(json);

    Streamy.LastColor = Streamy.RandomColor();

    self.name = json.name;
    self.thought = json.thought;
    self.date = json.date;
    self.date = new Date(self.date);
    self.color = Streamy.LastColor;
    self.seed = html ? true : false;
};

Streamy.Purge = function () {
    $.post('/thoughts/purge', function (result) {
        if (console)
            console.log(result);
    });
}