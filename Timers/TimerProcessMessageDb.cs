//using ThaiHungApi.Extensions;
//using Microsoft.EntityFrameworkCore;
//using Task = System.Threading.Tasks.Task;
//using System.Globalization;
//using ThaiHungApi.Databases.THSoft;

//using Microsoft.VisualStudio.Services.WebApi;


//namespace ThaiHungApi.Timers
//{
//    public class TimerProcessMessageDb : IHostedService, IDisposable
//    {
//        private readonly ILogger<TimerProcessMessageDb> _logger;
//        private Timer _timer;
//        private int TimeLoop = 2 * 1000; // ms

//        public TimerProcessMessageDb(ILogger<TimerProcessMessageDb> logger)
//        {
//            _logger = logger;
//        }

//        public void Dispose()
//        {
//            _timer?.Dispose();
//        }

//        /// <summary>
//        /// </summary>
//        /// <param name="cancellationToken"></param>
//        /// <returns></returns>
//        public Task StartAsync(CancellationToken cancellationToken)
//        {
//            ChangeStateTimer(true);
//            _logger.LogInformation("Processor message running.");
//            return Task.CompletedTask;
//        }

//        public Task StopAsync(CancellationToken cancellationToken)
//        {
//            _logger.LogInformation("Processor message is stopping.");
//            _timer?.Change(Timeout.Infinite, 0);
//            return Task.CompletedTask;
//        }

//        private async void ChangeStateTimer(bool enable)
//        {
//            _timer ??= new Timer(DoWork, null, Timeout.Infinite, Timeout.Infinite);

//            var time_wait = enable ? TimeLoop : Timeout.Infinite;
//            _timer.Change(0, time_wait);
//        }

//        private void DoWork(object state)
//        {
//            _timer.Change(Timeout.Infinite, Timeout.Infinite);

//            var _context = ServiceExtension.GetDbContext();

//            try
//            {
//                while (true)
//                {
//                    var msgQueue = MessageQueueManager.dequeue();

//                    if (msgQueue != null)
//                    {
//                        if (msgQueue.ListUsersToSendNotify != null && msgQueue.ListUsersToSendNotify.Count > 0)
//                        {
//                            //Gửi notify đến người dùng offline
//                            try
//                            {
//                                FirebaseCloudMessage.SendMulticastMessage(_context, msgQueue.ListUsersToSendNotify, msgQueue.ServerMsg).SyncResult();
//                            }
//                            catch (Exception ext)
//                            {
//                                _logger.LogError(ext.Message, ext);
//                            }
//                        }

//                        if (msgQueue.Content.Length > 2040)
//                        {
//                            msgQueue.Content = msgQueue.Content.Substring(0, 2040);
//                        }

//                        var msgGroup = _context.MessageRoom.Where(x => x.Uuid == msgQueue.MsgRoomUuid).SingleOrDefault();

//                        if (msgGroup != null)
//                        {
//                            msgGroup.LastMsgLineUuid = msgQueue.Uuid;

//                            var newMsgLine = new MessageLine
//                            {
//                                Uuid = msgQueue.Uuid,
//                                Content = msgQueue.Content,
//                                ContentType = msgQueue.ContentType,
//                                MsgRoomUuid = msgQueue.MsgRoomUuid,
//                                ReplyMsgUuid = msgQueue.ReplyMsgUuid,
//                                UserSent = msgQueue.UserSent,
//                                Status = 1
//                            };

//                            _context.MessageLine.Add(newMsgLine);

//                            _context.SaveChanges();

//                            //TODO: Add thông tin người xem vào bảng msg_read
//                        }
//                    }
//                    else
//                    {
//                        Thread.Sleep(500);
//                    }
//                }
//            }
//            finally
//            {
//                _context.Dispose();
//            }

//            _timer.Change(TimeLoop, TimeLoop);
//        }
//    }
