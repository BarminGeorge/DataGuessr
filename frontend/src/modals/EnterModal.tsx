
import { usePage } from "../PageContext";


export default function EnterModal() {
  const { setPage } = usePage();
  return (
    <div className="modal-global-container">
     
       <div className="modal-container">      
        <div className="centered-vertical-aligment">
       <div className="secondary-container">
    
       <div className="left-aligment">

       <img 
          src="src/assets/defaultavatar.jpg"
          alt="avatar"
          className="w-10 h-10 rounded-full border border-gray-300"
        />

        <input 
            type="text"
            className="text-input-primary"
            placeholder={"Введите логин"}
            />
       </div>
       </div>
       <div className="centered-aligment">
       <button className="button-primary">Продолжить как гость</button>; 
       </div>
       </div>
       </div>

        <div className="title-text">или</div>

       

        <div className="modal-container">  
        <div className="centered-vertical-aligment">       
        <div className="secondary-container">
        <p className="secondary-text">
            Войдите, для того чтобы:
            <ul>
              <li>Создавать лобби</li>
              <li>Сохранять статистику</li>
            </ul>
        </p>
       
        <div className="centered-aligment">
       <button className="button-primary" onClick={() => setPage("login")}>Войти</button>; 
       </div>
       </div>
        </div> 
    </div>

    </div>
  );
}
