# Attachment Storage Approach

For the architecture, I’d keep the backend focused on metadata only. Submissions are stored as JSON in a simple `FormEntry` table, and attachments have their own `Attachment` records with file name, size, content type, checksum, and a storage key. The API exposes the basic endpoints: creating submissions, listing and searching them, retrieving a single submission, listing attachments, and handling upload/download operations.

Given the size of the files and the expected number of submissions, I wouldn’t rely on local disk at all. Large uploads going through the web server would create too much I/O load, and the server would quickly become the bottleneck. It also makes scaling harder if the system ever needs to run on multiple instances.

Because of that, I’d prefer to depend on a cloud object-storage service. It solves durability and scaling problems and removes heavy file traffic from the API layer. The workflow is simple: the API issues short-lived upload URLs, and the client uploads directly to the storage service. After the upload completes, the client notifies the API so the metadata can be stored. Downloads follow the same pattern using signed URLs.

This keeps the backend clean, avoids performance issues, and handles large attachments and high submission volume without adding unnecessary complexity.
