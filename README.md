# LibreHardwareServer
A simple TCP server implementation of LibreHardwareHelper. This is mainly meant for IPC in <a href="https://github.com/waffle-lord/JASM" target="_blank">JASM</a> on windows computers.

# Connection
The server runs locally (`127.0.0.1`) and uses port `22528`. There is no reasoning for this, I chose it at random.

The server will keep a connection alive for 1 minute after a response is sent back to the client. The timeout is reset after each response is sent.


# Messages
Here are the messages the server can receive. Anything that isn't one of these messages will be sent back "invalid request".

All messages must use character 1 to denote the end of the message. <a href="https://github.com/waffle-lord/LibreHardwareServer/blob/main/LibreHardwareServer/ServerTests/Model/TestClient.cs#L21" target="_blank">Here is an example on the TestClient</a>.

| :envelope: **Message** | :clipboard: **Description**                      |
|---------|-----------------------------------------------------------------|
| cpu     | Gets the CPU data for the computer                              |
| memory  | Gets the memory data for the computer                           |
| gpu     | Gets the GPU data for the computer                              |
| ping    | Just a means to keep a connection alive without requesting data |
| close   | Tells the server to close the connection                        |
