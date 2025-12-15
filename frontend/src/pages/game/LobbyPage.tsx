import FormControl from '@mui/material/FormControl';
import FormControlLabel from '@mui/material/FormControlLabel';
import FormLabel from '@mui/material/FormLabel';
import Radio from '@mui/material/Radio';
import RadioGroup from '@mui/material/RadioGroup';
import PlayerCard from "../../components/Cards";
import Header from "../../components/Header";
import { gameHubService } from '../../apiUtils/HubServices';
import { RoomPrivacy, type PlayerDto, GameMode } from '../../apiUtils/dto';
import { usePage } from '../../PageContext';
import type { CurrentAppState } from '../../App';

function BpRadio(props: any) {
    return (
        <Radio
            disableRipple
            color="default"
            sx={{
                '& .MuiSvgIcon-root': {
                    fontSize: "1.5rem",
                },
            }}
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
                <FormControlLabel className="accent-text" disabled value="true_guess" control={<BpRadio />} label="Правда или нет" />
                <FormControlLabel className="accent-text" disabled value="random_guess" control={<BpRadio />} label="Случайный режим" />
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
                <FormControlLabel className="accent-text" value="60sec" control={<BpRadio />} label="60 секунд" />
                <FormControlLabel className="accent-text" value="90sec" control={<BpRadio />} label="90 секунд" />
            </RadioGroup>
        </FormControl>
    );
}




export default function LobbyPage(props: CurrentAppState) {
    const { setPage } = usePage();
    if (props.room == null) {
        alert("undefined error");
        setPage("home");
        return;
    }
    if (props.room.ownerId === props.user_id) {
        return (<LobbyPageCreatorView {...props} />);
    } else {
        return (<LobbyPageGuestView {...props} />);
    }

}


async function createGame(
    userId: string | null,
    roomId: string | undefined,
    setPage: (page: any) => void)
    {


    if (userId == null || !roomId) {
        alert("Не удалось начать игру");
        return;
    }
    const result = await gameHubService.createGame({
        userId: userId,
        roomId,
        mode: GameMode.Default,
        countQuestions: 5,
        questionDuration: 30
    });

    console.log(result);

    if (!result.success || !result.resultObj) {
        alert("Не удалось начать игру");
        return;
    }
    
    setPage("game_round");
}

function LobbyPageCreatorView(props: CurrentAppState) {
    const { setPage } = usePage();
    if (props.room == null) {
        alert("undefined error");
        setPage("home");
        return;
    }

    const playerList = props.room.players.map(
        player =>
            <PlayerCard variant="lobby-creator" username={player.playerName} action={() => alert("Игрок кикнут")}
            />);


    return (
        <div className="global-container">
            <Header variant="logo-and-avatar-and-interactive"
                interact_action={() => leaveRoom(props.user_id, props.room?.id, setPage, props.setRoom)}
                interact_label={"Leave lobby"}
            />
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
                            <div className="title-text-2">{props.room.id}</div>
                            <div className="settings-container">
                                <div className="settings-element">
                                    <CustomizedRadiosGameMode />
                                </div>
                                <div className="settings-element">
                                    <CustomizedRadiosGameTime />
                                </div>
                            </div>
                            <button className="button-primary"
                                onClick={() => createGame(props.user_id, props.room?.id, setPage)}>Начать</button>
                        </div>
                    </div>

                </div>
            </div>
        </div>
    );
}

async function leaveRoom(
    userId: string | null,
    roomId: string | undefined,
    setPage: (page: any) => void,
    setRoom: (x: any) => void) {


    if (userId == null || !roomId) {
        alert("Не удалось покинуть комнату");
        return;
    }
    const result = await gameHubService.leaveRoom({
        userId: userId,
        roomId,
    });
    console.log(result);

    if (!result.success) {
        alert("Не удалось покинуть комнату");
        return;
    }
    setRoom(null);
    setPage("home");
}

function LobbyPageGuestView(props: CurrentAppState) {
    const { setPage } = usePage();

    if (props.room == null) {
        alert("undefined error");
        setPage("home");
        return;
    }
    const playerList = props.room.players.map(player => <PlayerCard variant="lobby-common" username={player.playerName} />);

    return (
        <div className="global-container">
            <Header variant="logo-and-avatar-and-interactive"
                interact_action={() => leaveRoom(props.user_id, props.room?.id, setPage, props.setRoom)}
                interact_label={"Leave lobby"}
            />
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
                                <div className="picture-xl-container">
                                    <img
                                        src="src/assets/defaultavatar.jpg"
                                        className="default-picture"
                                    />
                                </div>
                                <button className="button-primary"
                                    disabled
                                    onClick={() => alert("Начали")}>Ждём начала</button>
                            </div>
                        </div>

                    </div>
                </div>
            </div>
        </div>
    );
}