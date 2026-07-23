from neomodel import StructuredNode, StringProperty, IntegerProperty, RelationshipTo, FloatProperty, \
    StructuredRel, RelationshipFrom


class LeadsToRel(StructuredRel):
    time = IntegerProperty(required=True)


class TransferRel(StructuredRel):
    time = IntegerProperty(required=True)


class TrainStation(StructuredNode):
    station_id = StringProperty(unique_index=True, required=True)
    city = StringProperty(required=True)
    name = StringProperty(unique_index=True, required=True)
    latitude = FloatProperty(required=True)
    longitude = FloatProperty(required=True)


class Stop(StructuredNode):
    stop_id = StringProperty(unique_index=True, required=True)
    start_station_time = StringProperty()
    arrival_time_minutes = IntegerProperty()
    arrival_time = StringProperty()
    departure_time_minutes = IntegerProperty()
    departure_time = StringProperty()
    station_id = StringProperty()
    train_composition_id = StringProperty()

    station = RelationshipTo('TrainStation', 'LOCATED_AT')

    leads_to = RelationshipTo('Stop', 'LEADS_TO', model=LeadsToRel)
    transfers = RelationshipTo('Stop', 'TRANSFER', model=TransferRel)
