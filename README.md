# GuestBook
Small Azure Cloud Services app that uses Azure Storage resources (blob containers, tables, queues)

Requirements: Implement a GuestBook cloud web application that accepts guest reviews (text) with images posted. 
- as users post reviews with images on the webpage – they are displayed in a chronological reversed order (with the more recent at the top of the page).
- the app should keep all posts and have them displayed at all times – while allowing guests to publish new posts.
- as guests could upload images of large different sizes – the guestbook app should handle image resizing (automatically) so that the image from each post is automatically scaled down to a standard small definition (around 128 pixels) as an icon.
- the original image is replaced in each post by the small resized icon of it.
- the original image is reachable by clicking the small icon in every post (this ensures that the guestbook page would not take too long to download and display in user browsers due to the large images from posts)
- You probably need to use: a table store for storing messages and link, a blob storage for storing images and thumbnails (icons), a web role for implementing the web page and a worker role that reads a queue storage having as entries large images to be scaled down. As the worker roles progresses through posts – their images are transformed to thumbnails and the web page updated to reflect that.
