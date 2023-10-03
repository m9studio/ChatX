function AddEvent()
{
	document.getElementById("InputMessage")
    .addEventListener("keyup", function(event) {
		event.preventDefault();
		if (event.keyCode === 13)
		{
			SendMessage();
		}
	});
}

function OpenWebsocket()
{
	let server = "ws://";
	if(InputServer.value == "")
	{
		InputServer.value = "localhost";
	}
	server += InputServer.value;
	if(InputPort.value == "")
	{
		InputPort.value = 21235;
	}
	server += ":" + InputPort.value;
	if(InputService.value == "")
	{
		InputService.value = "ChatX";
	}
	server += "/" + InputService.value;
	
	
	if(InputChat.value == "")
	{
		InputChat.value = "Chat";
	}
	if(InputName.value == "")
	{
		InputName.value = "User";
	}
	
	WS = new WebSocket(server);
	WS.onmessage = GetMessage;
	WS.onclose = OutChat;
	WS.onopen = function()
	{
		Send({
			"Type" : "Join", 
			"Name": Encode(InputName.value), 
			"Chat" : Encode(InputChat.value)
		});
	};
}


function JoinChat()
{
	OutChat();
	Code = InputCode;
	OpenWebsocket();
}
function OutChat()
{
	if(WS != null)
	{
		WS.onclose = function () {};
		WS.close();
		WS = null;
	}
	ListMessage.replaceChildren();
	ListUser.replaceChildren();
	Code = "";
}
function GetMessage(msg)
{
	let json = JSON.parse(msg.data);
	
	let bottom = (ListMessage.scrollTop + ListMessage.offsetHeight) == ListMessage.scrollHeight;
	
	switch(json.Type)
	{
		case "Message":
			AddMessage(json.Id, json.Name, json.Message);
			break;
		case "Join":
			JoinUser(json.Name);
			break;
		case "Out":
			OutUser(json.Name);
			break;
		case "List":
			AddUsers(json.List);
			break;
	}
	
	if(bottom)
	{
		ListMessage.scrollTo(0,ListMessage.scrollHeight);
	}
}
function Send(data)
{
	if(WS != null)
	{
		if(typeof data == "string")
		{
			WS.send(data);
		}
		else
		{
			WS.send(JSON.stringify(data));
		}
	}
}
function SendMessage()
{
	if(InputMessage.value != "")
	{
		Send(Encode(InputMessage.value));
		InputMessage.value = "";
	}
}
function AddMessage(id, name, text)
{
	let a = document.createElement("div");
	a.classList.add("message");
	let b = document.createElement("div");
	b.classList.add("name");
	b.innerText = Decode(name);
	if(b.innerText == InputName.value)
	{
		a.classList.add("message-my");
	}
	a.appendChild(b);
	a.appendChild(document.createElement("hr"));
	let c = document.createElement("div");
	c.innerText = Decode(text);
	a.appendChild(c);
	a.setAttribute("data-id", id);
	
	if(ListMessage.childElementCount > 0)
	{
		for(let i = 0; i < ListMessage.childElementCount; i++)
		{
			let n = ListMessage.children[ListMessage.childElementCount - i - 1];
			let n_id = n.getAttribute("data-id");
			if(id > n_id)
			{
				n.after(a);
				break;
			}
		}
	}
	else
	{
		ListMessage.appendChild(a);
	}
}
function JoinUser(name)
{
	let n = Decode(name);
	let b = document.createElement("div");
	b.classList.add("info");
	b.innerText = n + " joined the chat";
	ListMessage.appendChild(b);
	let a = document.createElement("div");
	a.classList.add("name");
	a.innerText = n;
	ListUser.appendChild(a);
}
function OutUser(name)
{
	let n = Decode(name);
	let b = document.createElement("div");
	b.classList.add("info");
	b.innerText = n + " outed the chat";
	ListMessage.appendChild(b);
	for(let i = 0; i < ListUser.childElementCount; i++)
	{
		if(ListUser.children[i].innerText == n)
		{
			ListUser.removeChild(ListUser.children[i]);
			break;
		}
	}
}
function AddUsers(users)
{
	let b = document.createElement("div");
	b.classList.add("info");
	b.innerText = "You are joined in to the chat [" + InputChat.value + "]";
	ListMessage.appendChild(b);
	for(let i = 0; i < users.length; i++)
	{
		let a = document.createElement("div");
		a.classList.add("name");
		a.innerText = Decode(users[i]);
		ListUser.appendChild(a);
	}
}
var WS = null;
var Code = "";