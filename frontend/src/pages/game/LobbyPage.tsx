import FormControl from '@mui/material/FormControl';
import FormControlLabel from '@mui/material/FormControlLabel';
import FormLabel from '@mui/material/FormLabel';
import Radio from '@mui/material/Radio';
import RadioGroup from '@mui/material/RadioGroup';
import PlayerCard from "../../components/Cards";
import Header from "../../components/Header";

function BpRadio(props: any) {
  return (
    <Radio
      disableRipple
      color="default"
      sx={{	
        '& .MuiSvgIcon-root': {
          fontSize: "1.5rem",
          },}}
      {...props}
    />
  );
}

function CustomizedRadiosGameMode() {
  return (
    <FormControl>
      <FormLabel id="game-mode-radios">Выберите режим игры</FormLabel>
      <RadioGroup
        defaultValue="time_guess"
        aria-labelledby="game-mode-radios"
        name="customized-radios"
      >
        <FormControlLabel className="accent-text" value="time_guess" control={<BpRadio />} label="Угадайте время" />
        <FormControlLabel className="accent-text" disabled value="true_guess"  control={<BpRadio />} label="Правда или нет" />
        <FormControlLabel className="accent-text" disabled  value="random_guess"  control={<BpRadio />} label="Случайный режим" />
      </RadioGroup>
    </FormControl>
  );
}


function CustomizedRadiosGameTime() {
  return (
    <FormControl>
      <FormLabel id="game-time-radios">Выберите время раунда</FormLabel>
      <RadioGroup
        defaultValue="60sec"
        aria-labelledby="game-time-radios"
        name="customized-radios"
      >
        <FormControlLabel className="accent-text" value="30sec" control={<BpRadio />} label="30 секунд" />
        <FormControlLabel className="accent-text" value="60sec"  control={<BpRadio />} label="60 секунд" />
        <FormControlLabel className="accent-text" value="90sec"  control={<BpRadio />} label="90 секунд" />
      </RadioGroup>
    </FormControl>
  );
}


export default function LobbyPage() {
  const playerList = [];  
  for (let i = 0; i < 10; i++) {
      playerList.push(<PlayerCard variant="lobby-common" username="Имя пользователя" />);
    }
  
  return (
    <div className="global-container">
      <Header variant="logo-and-avatar-and-leave-room"/>
            <div className="main-container">  

			<div className="row-template"> 
			<div className="list-container row-column">
              <div className="title-text-2">
              Игроки
              </div>
              {playerList}
            </div>

             
            <div className="down-picture row-column">
              <div className="centered-aligment">
				<div className="title-text-2">#SOIFL44</div>
                          <div className="settings-container">
                              <div className="settings-element">
                                  <CustomizedRadiosGameMode />
                              </div>
                              <div className="settings-element">
                                  <CustomizedRadiosGameTime />
                </div>
				</div>
				  <button className="button-primary" 
        			onClick={() => alert("Начали")}>Начать</button>
              </div>
            </div>
            
          </div>
		</div>
    </div>
  );
}