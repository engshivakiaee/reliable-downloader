# Reliable Downloader Exercise 🖥️

## Description

- Performing a normal GET on a file won't be reliable for two reasons. Firstly, we need to be able to recover from internet disconnections. Secondly, we need to not have to start from scratch every time, with intermittent internet disconnection and slow internet, it's unlikely we'll be able to download the whole file in one go.

- Luckily, some CDNs support downloading partial content so if we can get part of the way through, we can resume from this point.

- If the URL does not support partial content then we attempt to just download the whole file.

- Clinicians need to eventually receive new updates as they're sent out, even if it takes many attempts due to their internet connection. Your solution needs to recover from failures and should not exit until the file has been successfully downloaded.
