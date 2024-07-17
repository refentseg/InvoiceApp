const formatInvoiceDate = (dateString:string|Date) => {
	const dateObject = dateString instanceof Date ? dateString : new Date(dateString);

    // Check if dateObject is a valid date
    if (isNaN(dateObject.getTime())) {
        // Handle invalid date
        return "Invalid Date";
    }


	const options: Intl.DateTimeFormatOptions = { day: 'numeric', month: 'short', year: 'numeric' };
	const formattedDate = Intl.DateTimeFormat('en-ZA', options).format(new Date(dateString));

    return formattedDate;
  };
  export const formatter = new Intl.NumberFormat("en-ZA",{
    style:'currency',
    currency:'ZAR',
    minimumFractionDigits:2
  });

export function currencyFormat(amount:number){
    const formattedAmount = formatter.format(amount / 100);
    return formattedAmount
}
