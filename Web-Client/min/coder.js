function Encode(text)
{
	//return text;
	let coded = [];
	for(let i = 0; i < text.length; i++)
	{
		coded[i] = text.charCodeAt(i);
	}
	return JSON.stringify(coded);
}
function Decode(coded)
{
	//return coded;
	coded = JSON.parse(coded);
	let text = "";
	for(let i = 0; i < coded.length; i++)
	{
		text += String.fromCharCode(coded[i]);
	}
	
	return text;
}

/*
a = Encode("Hello world!");
(12) [72, 101, 108, 108, 111, 32, 119, 111, 114, 108, 100, 33]

Decode(a);
'Hello world!'
*/