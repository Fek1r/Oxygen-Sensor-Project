Check Version
 ```shell
 cloudflared --version
 ```

Login
```shell
cloudflared login
```

Create tunnel
```shell
cloudflared tunnel create <tunnel_name>
```

Connect tunnel and domain name
```shell
cloudflared tunnel route dns <tunnel_name> <domain_name>
```

  Quick tunnel
 ```bash
 Cloudflared tunnel --url http://localhost:8000
 ```
 
 Run tunnel
 ```shell
 cloudflared tunnel run <tunnel-name>
 ```

Tunnel List
```shell
cloudflared tunnel list
```

Delete Tunnel
```
cloudflared tunnel delete <tunnel-name>
```

Tunnel logs
```
cloudflared tunnel logs <tunnel-name>
```

Check tunnel Configuration
```
cloudflared tunnel info <tunnel-name>
```

Config file example `~/.cloudflared/config.yml`
```yml
tunnel: <tunnel-name>
credentials-file: /home/username/.cloudflared/<tunnel-id>.json

ingress:
  - hostname: api.example.com
    service: http://localhost:8000
  - hostname: app.example.com
    service: http://localhost:5000
  - service: http_status:404
```
