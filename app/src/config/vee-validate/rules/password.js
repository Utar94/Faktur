function isDigit(c) {
  return !isNaN(Number(c))
}
function isLetter(c) {
  return c.toLowerCase() !== c.toUpperCase()
}

export default function (value) {
  let digitCount = 0,
    lowerCount = 0,
    otherCount = 0,
    upperCount = 0
  for (const c of value) {
    if (isLetter(c)) {
      if (c.toLowerCase() === c) {
        lowerCount++
      } else {
        upperCount++
      }
    } else if (isDigit(c)) {
      digitCount++
    } else {
      otherCount++
    }
  }
  return digitCount && lowerCount && otherCount && upperCount
}
