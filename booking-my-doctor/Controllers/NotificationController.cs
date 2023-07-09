using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Mvc;
using Google.Cloud.Firestore;

[ApiController]
[Route("api/[controller]")]
public class NotificationsController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> SendNotification([FromBody] booking_my_doctor.Data.Entities.Notification notification)
    {
        // Khởi tạo FirebaseApp
        var pathToFirebaseConfig = "../FireBase/firebaseConfig.json";
        FirebaseApp.Create(new AppOptions
        {
            Credential = GoogleCredential.FromFile(pathToFirebaseConfig),
        });
        // Lấy tham chiếu đến Firestore database
        FirestoreDb db = FirestoreDb.Create("booking-my-doctor");

        // Lưu thông báo vào collection "notification"
        var collection = db.Collection("notification");
        var document = await collection.AddAsync(notification);

        // Lấy ID của thông báo vừa được tạo
        var notificationId = document.Id;

        return Ok(new { NotificationId = notificationId });

    }
}
