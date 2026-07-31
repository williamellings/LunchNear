window.lunchnearGeo = {
    getCurrentPosition: function () {
        return new Promise(function (resolve, reject) {
            if (!navigator.geolocation) {
                reject(new Error('Geolocation not supported'));
                return;
            }
            navigator.geolocation.getCurrentPosition(
                function (pos) {
                    resolve({
                        latitude: pos.coords.latitude,
                        longitude: pos.coords.longitude
                    });
                },
                function (err) { reject(err); },
                { enableHighAccuracy: true, timeout: 10000 }
            );
        });
    },
    getOrCreateUserId: function () {
        var key = 'lunchnear-user-id';
        var id = localStorage.getItem(key);
        if (!id) {
            id = crypto.randomUUID();
            localStorage.setItem(key, id);
        }
        return id;
    }
};
