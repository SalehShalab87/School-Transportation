# Planning worker boundary

Long-running planning jobs belong here when the PlanningRun lifecycle is defined.

Important: PlanningRun is not an outbox message. Persist planning work as its own job/domain concept. Use the outbox for durable integration events such as planning completion/publication notifications.
