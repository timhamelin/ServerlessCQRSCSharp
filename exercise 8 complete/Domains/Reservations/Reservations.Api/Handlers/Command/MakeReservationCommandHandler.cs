﻿using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Reservations.Domain.Commands;
using Reservations.Domain.Aggregates;
using CQRS.Essentials.Abstractions.CQRS;
using CQRS.Essentials.Abstractions.DDD;
using CQRS.Essentials.Abstractions.ES;

namespace Reservations.Api.Handlers.Command
{
    //virtual-workshop ex-6 hint
    public class MakeReservationCommandHandler : ICommandHandler<MakeReservation>
    {
        private readonly IAggregateFactory<Reservation> _reservationFactory;
        private readonly IEventStoreClient _eventStoreClient;

        public MakeReservationCommandHandler(IAggregateFactory<Reservation> reservationFactory, IEventStoreClient eventStoreClient)
        {
            _reservationFactory = reservationFactory;
            _eventStoreClient = eventStoreClient;
        }

        public async Task<object[]> Handle(MakeReservation command, CancellationToken cancellationToken)
        {
            var reservationId = command.Id;
            //use factory to get entity info
            var reservation = await _reservationFactory.Get(reservationId, cancellationToken);
            //do something on reservation to raise events
            var events = reservation.MakeReservation(command);
            //persist and publish
            await _eventStoreClient.Save(reservation, reservationId);
            //return events
            return events.ToArray();
        }
    }
}