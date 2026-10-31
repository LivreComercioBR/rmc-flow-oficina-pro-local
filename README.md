# RMC Flow Oficina Pro Local

Código-fonte da edição Local para Windows, compatível com x86 e x64.

## Backup-fonte inicial

O backup-fonte inicial está em `archive/rmc-flow-oficina-pro-local-source.zip.b64.part-*`.

Para reconstruir o ZIP em Linux/macOS:

```bash
cat archive/rmc-flow-oficina-pro-local-source.zip.b64.part-* | base64 -d > rmc-flow-oficina-pro-local-source.zip
```

O arquivo contém a solução Visual Studio, código C#, instaladores Inno Setup e documentação. A chave pública de validação pode ser distribuída; nenhuma chave privada de assinatura ou licença emitida está incluída.

## Segurança

Nunca envie a chave RSA privada, bancos SQLite de clientes, licenças assinadas ou dados comerciais de clientes.
