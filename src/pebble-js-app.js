/*
 * pebble-js-app.js
 * Sends the sample message once it is initialized.
 */

function fetchWhereIsXur() {
  var req = new XMLHttpRequest();
  req.open('GET', 'http://whereisxur.azurewebsites.net/api/whereisxur', true);
  req.onload = function () {
    if (req.readyState === 4) {
      if (req.status === 200) {
        console.log(req.responseText);
        var text = req.responseText;
        Pebble.sendAppMessage({
          'WHERE_IS_XUR': text          
        });
      } else {
        console.log('Error');
      }
    }
  };
  req.send(null);
}

Pebble.addEventListener('ready', function(e) {
  console.log('PebbleKit JS Ready!');
  fetchWhereIsXur();
});