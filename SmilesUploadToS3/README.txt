[Frequency]

<add key="BackupFreqency" value="ByMinute" />
<add key="BackupValue" value="1,0" />

Code Examples

Run every second on the second.
TickTimer.Events.Add(new  Schedule.ScheduledTime("BySecond",  "0"));

Run every minute 15 seconds after the second.
TickTimer.Events.Add(new  Schedule.ScheduledTime("ByMinute",  "15,0"));

Run at 6:00 AM on Mondays.
TickTimer.Events.Add(new  Schedule.ScheduledTime("Weekly",  "1,6:00AM"));

Run once at 6:00 AM on 6/27/08.
TickTimer.Events.Add(new  Schedule.SingleEvent(new DateTime("6/27/2008 6:00")));

Run every 12 minutes starting on midnight 1/1/2003.
TickTimer.Events.Add(new Schedule.SimpleInterval(new 
           DateTime("1/1/2003"), TimeSpan.FromMinutes(12)));

Run every 15 minutes from 6:00 AM to 5:00 PM.
TickTimer.Events.Add(
    new Schedule.BlockWrapper(
        new Schedule.SimpleInterval(new DateTime("1/1/2003"),
                                           TimeSpan.FromMinutes(15)),
        "Daily",
        "6:00 AM",
        "5:00 PM"
    )
);


[Region]
US East (Virginia)        = USEast1
US West (N. California)   = USWest1
US West (Oregon)          = USWest2
EU West (Ireland)         = EUWest1
EU Central (Frankfurt)
Asia Pacific (Tokyo)      = APNortheast1
Asia Pacific (Seoul)
Asia Pacific (Singapore)  = APSoutheast1
Asia Pacific (Sydney)     = APSoutheast2
South America (Sao Paulo) = SAEast1
US GovCloud West (Oregon) = USGovCloudWest1
China (Beijing)

Mode 3 mode
1."sssdoc" (27.254.238.180)
2."test"  (192.168.100.50)
3."smiledoc" (192.168.100.9) 


