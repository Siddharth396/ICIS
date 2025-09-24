namespace Infrastructure.PubNub
{
    using System.Threading.Tasks;

    using Infrastructure.PubNub.Models;

    public interface IPubNubNotificationService
    {
        void AddPubNubNotification(PubNubData<PriceItemEvent> data);

        Task SendPubNubNotifications();
    }
}
