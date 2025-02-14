# Beefweb.Client API

Beefweb.Client is a client library for interacting with [Beefweb](http://github.com/hyperblast/beefweb) enabled players.

## Basic usage

```cs
using var client = new PlayerClient("http://localhost:8880");

await client.Play();
```
