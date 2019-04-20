# AggregateVsTolookup
  
因為 ToLookUp() 呼叫了外部 variable  
所以想找到不需要存取外部 variable  
又要達到一樣功能的做法

即使是透過 aggreate 操作 Dictionary<int, List<T>> ，效能也不輸 ILook<TKey,TValue>  
