# Beefweb.Client API

Beefweb.Client is a client library for interacting with [Beefweb](http://github.com/hyperblast/beefweb) enabled players.

## Basic usage

```cs
// Create client
var client = new PlayerClient(new Uri("http://localhost:8880"));

// Play first track of current playlist
await client.Play(PlaylistRef.Current, 0);
```
