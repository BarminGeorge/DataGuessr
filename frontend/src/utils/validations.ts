
export function validatePassword(password: string) : string {
  if (password.length === 0) { return " "; }
  if (password.length < 6) {
    return "Пароль должен быть не менее 6 символов";
  } else if (!/[A-Z]/.test(password)) {
    return "Пароль должен содержать хотя бы одну заглавную букву";
  } else if (!/[a-z]/.test(password)) {
    return "Пароль должен содержать хотя бы одну строчную букву";
  } else if (!/[0-9]/.test(password)) {
    return "Пароль должен содержать хотя бы одну цифру";
  }
  return "";
}

export function validateLogin(login: string) : string {
  if (login.length === 0) { return " "; }
  if (login.length < 3) {
    return "Логин должен быть не менее 3 символов";
  } else if (login.length > 50) {
    return "Логин должен быть не более 50 символов";
  } else if (/^[a-zA-Z0-9_]+$/.test(login) === false) {
    return "Логин может содержать только буквы, цифры и символы подчеркивания";
  } 
  return "";
}

export function validateUsername(name: string) : string {
  if (name.length === 0) { return " "; }
  if (name.length < 2) {
    return "Имя должено быть не менее 2 символов";
  } else if (name.length > 50) {
    return "Имя должено быть не более 50 символов";
  } else if (/^[a-zA-Z0-9_ ]+$/.test(name) === false) {
    return "Логин может содержать только буквы, цифры, пробелы и символы подчеркивания";
  } 
  return "";
}