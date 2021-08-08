export const logIn = async (username: string, password: string) => {
	return new Promise((resolve, reject) => {
		setTimeout(() => {
			resolve('TOKEN');
		}, 1000);
	});
};
export const logOut = async () => {};
