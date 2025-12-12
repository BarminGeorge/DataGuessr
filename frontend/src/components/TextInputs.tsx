type TextProps = {
  Text: string;
};

function TextInput({Text}: TextProps){
    
  return <input 
  type="text"
  className="text-input-primary"
  placeholder={Text}
  
  ></input>;
}

export default TextInput;