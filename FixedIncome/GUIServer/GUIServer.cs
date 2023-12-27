using FlatbufferOnServer;
using FixedIncomeManager;
using FixedIncomeManager.Models;
using FixedIncomeProtocol;
using FlatBuffers;

namespace FixedIncome.GUIServer
{
    internal class GUIServer : MainServer<FixedIncomeMessageType, FixedIncomeMessage>
    {
        private readonly Manager _manager;
        internal GUIServer(Manager manager)
            : base()
        {
            _manager = manager;
        }
        protected override void FillCallbacks()
        {
            Callbacks[FixedIncomeMessageType.RequestIndexers] = OnRequestFixedIncome;
            Callbacks[FixedIncomeMessageType.RequestFixedIncomes] = OnRequestIndexer;
        }
        protected override byte[] GetMessageBytes(FixedIncomeMessageType type, object message)
        {
            FixedIncomeMessageTypeUnion msgUnion = new FixedIncomeMessageTypeUnion
            {
                Type = type,
                Value = message
            };
            FixedIncomeMessageT msgFlatBuffer = new FixedIncomeMessageT
            {
                Message = msgUnion,
            };

            FlatBufferBuilder builder = new FlatBufferBuilder(1);
            var offset = FixedIncomeMessage.Pack(
                builder,
                msgFlatBuffer);
            builder.Finish(offset.Value);
            return builder.DataBuffer.ToSizedArray();
        }
        protected override FixedIncomeMessage GetMessageFromBytes(byte[] messageBytes)
        {
            return FixedIncomeMessage.GetRootAsFixedIncomeMessage(new ByteBuffer(messageBytes));
        }
        protected override bool IsValidMessage(FixedIncomeMessage message)
        {
            return Callbacks.ContainsKey(message.MessageType);
        }
        protected override void ProcessMessage(ProtocolControl clientSocket, FixedIncomeMessage message)
        {
            Callbacks[message.MessageType].Invoke(clientSocket, message);
        }
        private void OnRequestFixedIncome(ProtocolControl clientSocket, FixedIncomeMessage message)
        {
            ResponseFixedIncomesT response = GetResponse(_manager.Get());
            clientSocket.Send(PackMessage(FixedIncomeMessageType.ResponseFixedIncomes, response));
        }
        private void OnRequestIndexer(ProtocolControl clientSocket, FixedIncomeMessage message)
        {
            //TODO
        }
        private ResponseFixedIncomesT GetResponse(List<FixedIncomeModel> fixedIncomes)
        {
            ResponseFixedIncomesT response = new ResponseFixedIncomesT();
            foreach(var item in fixedIncomes)
            {
                response.Itens.Add(new FixedIncomeT()
                {
                    BeginDate = (ulong)item.Hiring.Ticks,
                    Capital = item.Capital,
                    FixedIncomeType = (FixedIncomeProtocol.FixedIncomeType)item.Type,
                    Indexer = (FixedIncomeProtocol.IndexerType)item.Indexer,
                    MaturityDate = (ulong)item.Expiration.Ticks,
                    Name = item.Name,
                    Remuneration = (float)item.Remuneration,
                    TaxType = (FixedIncomeProtocol.TaxType)item.TaxType
                });
            }
            return response;
        }

    }
}
