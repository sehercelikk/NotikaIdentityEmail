﻿namespace NotikaIdentityEmail.Entities;

public class Message
{
    public int MessageId { get; set; }
    public string SenderEmail { get; set; }
    public string ReceiveEmail { get; set; }
    public string Konu { get; set; }
    public DateTime SendDate { get; set; }
    public string MessageDetail { get; set; }
    public bool IsRead { get; set; }
    public int CategoryId { get; set; }
    public Category Category { get; set; }

}
