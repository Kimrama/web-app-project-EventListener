namespace EventListener.Models;

public class ChatroomListFromActivity {
    public required string ActivityName {get; set;}
    public required string ActivityOwnerId {get; set;}
    public required DateTime ActivityCreateAt {get; set;}
    public required string RoomIdHash {get; set;}
}