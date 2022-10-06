## Disclaimer
This application has been made as a learning project. I won't be responsible for any damage caused by its usage.

# PswManager
PswManager handles the encryption and storage of passwords locally.

They are stored as "accounts", objects with values of a Name, a Password, and an Email. The first remains uncrypted and is used as a primary key, while the other two are encrypted through two passwords generated from a master key. Authentication of the master key is done through a token file.

Currently, four type of databases are supported: SQLite, Json, Text, and a memory database for testing.

It is possible to boot the application through either the console or a WPF window.
