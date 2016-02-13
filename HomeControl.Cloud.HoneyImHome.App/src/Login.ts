export class Login  {
    heading: string = 'Login';
    firstName: string = 'John';
    lastName: string = 'Doe';

    get fullName(): string {
    return `${ this.firstName } ${ this.lastName}`;
    }

    submit(): void {
        alert(`Welcome, ${this.fullName }!`);
    }
}