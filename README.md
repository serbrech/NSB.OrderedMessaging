# NSB.OrderedMessaging

## Disclaimer
This is not production code. it is at the proof of concept stage :). Do what you please with it, but don't blame me.  

Using the principles of lamport timestamp to ensure ordered processing.  
We set a sequence number in the header of the messages.  
if the messages arrive out of order on the receiver, we throw an exception and send it back to the Retry queue according to the recovery settings.  


```cs
    EndpointConfiguration config = new EndpointConfiguration("enpoint.subscriber");
    config.UseOrderedMessaging(typeof(IBookSold));
```


