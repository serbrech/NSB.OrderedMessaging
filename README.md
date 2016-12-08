# NSB.OrderedMessaging

## Disclaimer
###This is not production code. 
it is at the proof of concept stage :). Do what you please with it, but don't blame me.  

##What is it?
Using the principles of lamport timestamp to ensure ordered processing.  
We set a sequence number in the header of the messages.  
If the messages arrive out of order on the receiver, we throw an exception and send it back to the Retry queue according to the recovery settings.  

```cs
    EndpointConfiguration config = new EndpointConfiguration("enpoint.subscriber");
    config.UseOrderedMessaging(typeof(IBookSold));
```

##Is this useful?
Not really... or well, if you have a single endpoint per queue, it might be. Don't expect it to work if you scale out.

