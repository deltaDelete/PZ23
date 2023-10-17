create table if not exists clients
(
    client_id   int auto_increment
        primary key,
    last_name   varchar(255) charset utf8mb3 not null,
    first_name  varchar(255) charset utf8mb3 not null,
    middle_name varchar(255) charset utf8mb3 null
)
    charset = utf8mb4;

create table if not exists executors
(
    executor_id int auto_increment
        primary key,
    last_name   varchar(255) charset utf8mb3 not null,
    first_name  varchar(255) charset utf8mb3 not null,
    middle_name varchar(255) charset utf8mb3 null
)
    charset = utf8mb4;

create table if not exists failure_types
(
    failure_type_id   int auto_increment
        primary key,
    failure_type_name varchar(255) charset utf8mb3 not null
)
    charset = utf8mb4;

create table if not exists gears
(
    gear_id   int auto_increment
        primary key,
    gear_name varchar(255) charset utf8mb3 not null
)
    charset = utf8mb4;

create table if not exists `groups`
(
    group_id    int auto_increment
        primary key,
    group_name  varchar(255) charset utf8mb3 not null,
    permissions int                          not null comment 'Enum флаг'
);

create table if not exists priorities
(
    priority_id   int auto_increment
        primary key,
    priority_name varchar(255) charset utf8mb3 not null
)
    charset = utf8mb4;

create table if not exists request_statuses
(
    request_status_id   int auto_increment
        primary key,
    request_status_name varchar(255) charset utf8mb3 not null
)
    charset = utf8mb4;

create table if not exists requests
(
    request_id          int auto_increment
        primary key,
    start_date          date                          not null,
    gear_id             int                           not null,
    failure_type_id     int                           not null,
    failure_description varchar(1024) charset utf8mb3 null,
    client_id           int                           not null,
    priority_id         int                           not null,
    request_status_id   int                           not null,
    constraint requests_clients_client_id_fk
        foreign key (client_id) references clients (client_id),
    constraint requests_failure_types_failure_type_id_fk
        foreign key (failure_type_id) references failure_types (failure_type_id),
    constraint requests_gears_gear_id_fk
        foreign key (gear_id) references gears (gear_id),
    constraint requests_priorities_priority_id_fk
        foreign key (priority_id) references priorities (priority_id),
    constraint requests_request_statuses_request_status_id_fk
        foreign key (request_status_id) references request_statuses (request_status_id)
)
    charset = utf8mb4;

create table if not exists execution
(
    execution_id int auto_increment
        primary key,
    executor_id  int not null,
    request_id   int not null,
    constraint execution_executors_executor_id_fk
        foreign key (executor_id) references executors (executor_id),
    constraint execution_requests_request_id_fk
        foreign key (request_id) references requests (request_id)
)
    charset = utf8mb4;

create table if not exists services
(
    service_id    int auto_increment
        primary key,
    service_name  varchar(255) charset utf8mb3 not null,
    service_price decimal                      not null
)
    charset = utf8mb4;

create table if not exists request_services
(
    request_services_id int auto_increment
        primary key,
    request_id          int not null,
    service_id          int not null,
    constraint request_services_requests_request_id_fk
        foreign key (request_id) references requests (request_id),
    constraint request_services_services_service_id_fk
        foreign key (service_id) references services (service_id)
)
    charset = utf8mb4;

create table if not exists users
(
    user_id  int auto_increment
        primary key,
    username varchar(255) charset utf8mb3 not null,
    password varchar(255) charset utf8mb3 not null
);

create table if not exists user_groups
(
    user_id  int not null,
    group_id int not null,
    primary key (user_id, group_id),
    constraint user_groups_ibfk_1
        foreign key (group_id) references `groups` (group_id),
    constraint user_groups_ibfk_2
        foreign key (user_id) references users (user_id)
);

create index if not exists group_id
    on user_groups (group_id);


