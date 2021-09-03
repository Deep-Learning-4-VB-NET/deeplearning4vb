Imports System
Imports System.Collections.Generic
Imports System.Text
Imports RandomGenerator = org.apache.commons.math3.random.RandomGenerator
Imports FastMath = org.apache.commons.math3.util.FastMath
Imports org.nd4j.common.primitives

'
' *  ******************************************************************************
' *  *
' *  *
' *  * This program and the accompanying materials are made available under the
' *  * terms of the Apache License, Version 2.0 which is available at
' *  * https://www.apache.org/licenses/LICENSE-2.0.
' *  *
' *  *  See the NOTICE file distributed with this work for additional
' *  *  information regarding copyright ownership.
' *  * Unless required by applicable law or agreed to in writing, software
' *  * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
' *  * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
' *  * License for the specific language governing permissions and limitations
' *  * under the License.
' *  *
' *  * SPDX-License-Identifier: Apache-2.0
' *  *****************************************************************************
' 

Namespace org.nd4j.common.util




	Public Class MathUtils



		''' <summary>
		''' The natural logarithm of 2.
		''' </summary>
'JAVA TO VB CONVERTER NOTE: The field log2 was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Public Shared ReadOnly log2_Conflict As Double = Math.Log(2)
		''' <summary>
		''' The small deviation allowed in double comparisons.
		''' </summary>
		Public Const SMALL As Double = 1e-6


		Public Shared Function pow(ByVal base As Double, ByVal exponent As Double) As Double
			Dim result As Double = 1

			If exponent = 0 Then
				Return result
			End If
			If exponent < 0 Then
				Return 1 / pow(base, exponent * -1)
			End If

			Return FastMath.pow(base, exponent)
		End Function

		''' <summary>
		''' Normalize a value
		''' (val - min) / (max - min)
		''' </summary>
		''' <param name="val"> value to normalize </param>
		''' <param name="max"> max value </param>
		''' <param name="min"> min value </param>
		''' <returns> the normalized value </returns>
		Public Shared Function normalize(ByVal val As Double, ByVal min As Double, ByVal max As Double) As Double
			If max < min Then
				Throw New System.ArgumentException("Max must be greather than min")
			End If

			Return (val - min) / (max - min)
		End Function

		''' <summary>
		''' Clamps the value to a discrete value
		''' </summary>
		''' <param name="value"> the value to clamp </param>
		''' <param name="min">   min for the probability distribution </param>
		''' <param name="max">   max for the probability distribution </param>
		''' <returns> the discrete value </returns>
		Public Shared Function clamp(ByVal value As Integer, ByVal min As Integer, ByVal max As Integer) As Integer
			If value < min Then
				value = min
			End If
			If value > max Then
				value = max
			End If
			Return value
		End Function

		''' <summary>
		''' Discretize the given value
		''' </summary>
		''' <param name="value">    the value to discretize </param>
		''' <param name="min">      the min of the distribution </param>
		''' <param name="max">      the max of the distribution </param>
		''' <param name="binCount"> the number of bins </param>
		''' <returns> the discretized value </returns>
		Public Shared Function discretize(ByVal value As Double, ByVal min As Double, ByVal max As Double, ByVal binCount As Integer) As Integer
			Dim discreteValue As Integer = CInt(Math.Truncate(binCount * normalize(value, min, max)))
			Return clamp(discreteValue, 0, binCount - 1)
		End Function

		''' <summary>
		''' See: <a href="https://stackoverflow.com/questions/466204/rounding-off-to-nearest-power-of-2">https://stackoverflow.com/questions/466204/rounding-off-to-nearest-power-of-2</a>
		''' </summary>
		''' <param name="v"> the number to getFromOrigin the next power of 2 for </param>
		''' <returns> the next power of 2 for the passed in value </returns>
		Public Shared Function nextPowOf2(ByVal v As Long) As Long
			v -= 1
			v = v Or v >> 1
			v = v Or v >> 2
			v = v Or v >> 4
			v = v Or v >> 8
			v = v Or v >> 16
			v += 1
			Return v

		End Function

		''' <summary>
		''' Generates a binomial distributed number using
		''' the given rng
		''' </summary>
		''' <param name="rng"> </param>
		''' <param name="n"> </param>
		''' <param name="p">
		''' @return </param>
		Public Shared Function binomial(ByVal rng As RandomGenerator, ByVal n As Integer, ByVal p As Double) As Integer
			If (p < 0) OrElse (p > 1) Then
				Return 0
			End If
			Dim c As Integer = 0
			For i As Integer = 0 To n - 1
				If rng.nextDouble() < p Then
					c += 1
				End If
			Next i
			Return c
		End Function

		''' <summary>
		''' Generate a uniform random number from the given rng
		''' </summary>
		''' <param name="rng"> the rng to use </param>
		''' <param name="min"> the min num </param>
		''' <param name="max"> the max num </param>
		''' <returns> a number uniformly distributed between min and max </returns>
		Public Shared Function uniform(ByVal rng As Random, ByVal min As Double, ByVal max As Double) As Double
			Return rng.NextDouble() * (max - min) + min
		End Function

		''' <summary>
		''' Returns the correlation coefficient of two double vectors.
		''' </summary>
		''' <param name="residuals">       residuals </param>
		''' <param name="targetAttribute"> target attribute vector </param>
		''' <returns> the correlation coefficient or r </returns>
		Public Shared Function correlation(ByVal residuals() As Double, ByVal targetAttribute() As Double) As Double
			Dim predictedValues(residuals.Length - 1) As Double
			For i As Integer = 0 To predictedValues.Length - 1
				predictedValues(i) = targetAttribute(i) - residuals(i)
			Next i
			Dim ssErr As Double = ssError(predictedValues, targetAttribute)
			Dim total As Double = ssTotal(residuals, targetAttribute)
			Return 1 - (ssErr / total)
		End Function 'end correlation

		''' <summary>
		''' 1 / 1 + exp(-x)
		''' </summary>
		''' <param name="x">
		''' @return </param>
		Public Shared Function sigmoid(ByVal x As Double) As Double
			Return 1.0 / (1.0 + Math.Pow(Math.E, -x))
		End Function

		''' <summary>
		''' How much of the variance is explained by the regression
		''' </summary>
		''' <param name="residuals">       error </param>
		''' <param name="targetAttribute"> data for target attribute </param>
		''' <returns> the sum squares of regression </returns>
		Public Shared Function ssReg(ByVal residuals() As Double, ByVal targetAttribute() As Double) As Double
			Dim mean As Double = sum(targetAttribute) / targetAttribute.Length
			Dim ret As Double = 0
			For i As Integer = 0 To residuals.Length - 1
				ret += Math.Pow(residuals(i) - mean, 2)
			Next i
			Return ret
		End Function

		''' <summary>
		''' How much of the variance is NOT explained by the regression
		''' </summary>
		''' <param name="predictedValues"> predicted values </param>
		''' <param name="targetAttribute"> data for target attribute </param>
		''' <returns> the sum squares of regression </returns>
		Public Shared Function ssError(ByVal predictedValues() As Double, ByVal targetAttribute() As Double) As Double
			Dim ret As Double = 0
			For i As Integer = 0 To predictedValues.Length - 1
				ret += Math.Pow(targetAttribute(i) - predictedValues(i), 2)
			Next i
			Return ret
		End Function

		''' <summary>
		''' Calculate string similarity with tfidf weights relative to each character
		''' frequency and how many times a character appears in a given string </summary>
		''' <param name="strings"> the strings to calculate similarity for </param>
		''' <returns> the cosine similarity between the strings </returns>
		Public Shared Function stringSimilarity(ParamArray ByVal strings() As String) As Double
			If strings Is Nothing Then
				Return 0
			End If
			Dim counter As New Counter(Of String)()
			Dim counter2 As New Counter(Of String)()

			Dim i As Integer = 0
			Do While i < strings(0).Length
				counter.incrementCount((strings(0).Chars(i)).ToString(), 1.0f)
				i += 1
			Loop

			i = 0
			Do While i < strings(1).Length
				counter2.incrementCount((strings(1).Chars(i)).ToString(), 1.0f)
				i += 1
			Loop
			Dim v1 As ISet(Of String) = counter.keySet()
			Dim v2 As ISet(Of String) = counter2.keySet()


			Dim both As ISet(Of String) = SetUtils.intersection(v1, v2)

			Dim sclar As Double = 0, norm1 As Double = 0, norm2 As Double = 0
			For Each k As String In both
				sclar += counter.getCount(k) * counter2.getCount(k)
			Next k
			For Each k As String In v1
				norm1 += counter.getCount(k) * counter.getCount(k)
			Next k
			For Each k As String In v2
				norm2 += counter2.getCount(k) * counter2.getCount(k)
			Next k
			Return sclar / Math.Sqrt(norm1 * norm2)
		End Function

		''' <summary>
		''' Returns the vector length (sqrt(sum(x_i))
		''' </summary>
		''' <param name="vector"> the vector to return the vector length for </param>
		''' <returns> the vector length of the passed in array </returns>
		Public Shared Function vectorLength(ByVal vector() As Double) As Double
			Dim ret As Double = 0
			If vector Is Nothing Then
				Return ret
			Else
				For i As Integer = 0 To vector.Length - 1
					ret += Math.Pow(vector(i), 2)
				Next i

			End If
			Return ret
		End Function

		''' <summary>
		''' Inverse document frequency: the total docs divided by the number of times the word
		''' appeared in a document
		''' </summary>
		''' <param name="totalDocs">                       the total documents for the data applyTransformToDestination </param>
		''' <param name="numTimesWordAppearedInADocument"> the number of times the word occurred in a document </param>
		''' <returns> log(10) (totalDocs/numTImesWordAppearedInADocument) </returns>
		Public Shared Function idf(ByVal totalDocs As Double, ByVal numTimesWordAppearedInADocument As Double) As Double
			Return If(totalDocs > 0, Math.Log10(totalDocs / numTimesWordAppearedInADocument), 0)
		End Function

		''' <summary>
		''' Term frequency: 1+ log10(count)
		''' </summary>
		''' <param name="count"> the count of a word or character in a given string or document </param>
		''' <returns> 1+ log(10) count </returns>
		Public Shared Function tf(ByVal count As Integer) As Double
			Return If(count > 0, 1 + Math.Log10(count), 0)
		End Function

		''' <summary>
		''' Return td * idf
		''' </summary>
		''' <param name="td">  the term frequency (assumed calculated) </param>
		''' <param name="idf"> inverse document frequency (assumed calculated) </param>
		''' <returns> td * idf </returns>
		Public Shared Function tfidf(ByVal td As Double, ByVal idf As Double) As Double
			Return td * idf
		End Function

		Private Shared Function charForLetter(ByVal c As Char) As Integer
			Dim chars() As Char = {"a"c, "b"c, "c"c, "d"c, "e"c, "f"c, "g"c, "h"c, "i"c, "j"c, "k"c, "l"c, "m"c, "n"c, "o"c, "p"c, "q"c, "r"c, "s"c, "t"c, "u"c, "v"c, "w"c, "x"c, "y"c, "z"c}
			For i As Integer = 0 To chars.Length - 1
				If chars(i) = c Then
					Return i
				End If
			Next i
			Return -1

		End Function

		''' <summary>
		''' Total variance in target attribute
		''' </summary>
		''' <param name="residuals">       error </param>
		''' <param name="targetAttribute"> data for target attribute </param>
		''' <returns> Total variance in target attribute </returns>
		Public Shared Function ssTotal(ByVal residuals() As Double, ByVal targetAttribute() As Double) As Double
			Return ssReg(residuals, targetAttribute) + ssError(residuals, targetAttribute)
		End Function

		''' <summary>
		''' This returns the sum of the given array.
		''' </summary>
		''' <param name="nums"> the array of numbers to sum </param>
		''' <returns> the sum of the given array </returns>
		Public Shared Function sum(ByVal nums() As Double) As Double

			Dim ret As Double = 0
			For Each d As Double In nums
				ret += d
			Next d

			Return ret
		End Function 'end sum

		''' <summary>
		''' This will merge the coordinates of the given coordinate system.
		''' </summary>
		''' <param name="x"> the x coordinates </param>
		''' <param name="y"> the y coordinates </param>
		''' <returns> a vector such that each (x,y) pair is at ret[i],ret[i+1] </returns>
		Public Shared Function mergeCoords(ByVal x() As Double, ByVal y() As Double) As Double()
			If x.Length <> y.Length Then
				Throw New System.ArgumentException("Sample sizes must be the same for each data applyTransformToDestination.")
			End If
			Dim ret((x.Length + y.Length) - 1) As Double

			For i As Integer = 0 To x.Length - 1
				ret(i) = x(i)
				ret(i + 1) = y(i)
			Next i
			Return ret
		End Function 'end mergeCoords

		''' <summary>
		''' This will merge the coordinates of the given coordinate system.
		''' </summary>
		''' <param name="x"> the x coordinates </param>
		''' <param name="y"> the y coordinates </param>
		''' <returns> a vector such that each (x,y) pair is at ret[i],ret[i+1] </returns>
		Public Shared Function mergeCoords(ByVal x As IList(Of Double), ByVal y As IList(Of Double)) As IList(Of Double)
			If x.Count <> y.Count Then
				Throw New System.ArgumentException("Sample sizes must be the same for each data applyTransformToDestination.")
			End If

			Dim ret As IList(Of Double) = New List(Of Double)()

			For i As Integer = 0 To x.Count - 1
				ret.Add(x(i))
				ret.Add(y(i))
			Next i
			Return ret
		End Function 'end mergeCoords

		''' <summary>
		''' This returns the minimized loss values for a given vector.
		''' It is assumed that  the x, y pairs are at
		''' vector[i], vector[i+1]
		''' </summary>
		''' <param name="vector"> the vector of numbers to getFromOrigin the weights for </param>
		''' <returns> a double array with w_0 and w_1 are the associated indices. </returns>
		Public Shared Function weightsFor(ByVal vector As IList(Of Double)) As Double()
			' split coordinate system 
			Dim coords As IList(Of Double()) = coordSplit(vector)
			' x vals 
			Dim x() As Double = coords(0)
			' y vals 
			Dim y() As Double = coords(1)


			Dim meanX As Double = sum(x) / x.Length
			Dim meanY As Double = sum(y) / y.Length

			Dim sumOfMeanDifferences As Double = MathUtils.sumOfMeanDifferences(x, y)
			Dim xDifferenceOfMean As Double = sumOfMeanDifferencesOnePoint(x)

			Dim w_1 As Double = sumOfMeanDifferences / xDifferenceOfMean

			Dim w_0 As Double = meanY - (w_1) * meanX

			'double w_1=(n*sumOfProducts(x,y) - sum(x) * sum(y))/(n*sumOfSquares(x) - Math.pow(sum(x),2));

			'	double w_0=(sum(y) - (w_1 * sum(x)))/n;

			Dim ret(vector.Count - 1) As Double
			ret(0) = w_0
			ret(1) = w_1

			Return ret
		End Function 'end weightsFor

		''' <summary>
		''' This will return the squared loss of the given
		''' points
		''' </summary>
		''' <param name="x">   the x coordinates to use </param>
		''' <param name="y">   the y coordinates to use </param>
		''' <param name="w_0"> the first weight </param>
		''' <param name="w_1"> the second weight </param>
		''' <returns> the squared loss of the given points </returns>
		Public Shared Function squaredLoss(ByVal x() As Double, ByVal y() As Double, ByVal w_0 As Double, ByVal w_1 As Double) As Double
			Dim sum As Double = 0
			For j As Integer = 0 To x.Length - 1
				sum += Math.Pow((y(j) - (w_1 * x(j) + w_0)), 2)
			Next j
			Return sum
		End Function 'end squaredLoss

		Public Shared Function w_1(ByVal x() As Double, ByVal y() As Double, ByVal n As Integer) As Double
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
			Return (n * sumOfProducts(x, y) - sum(x) * sum(y)) / (n * sumOfSquares(x) - Math.Pow(sum(x), 2))
		End Function

		Public Shared Function w_0(ByVal x() As Double, ByVal y() As Double, ByVal n As Integer) As Double
			Dim weight1 As Double = w_1(x, y, n)

			Return (sum(y) - (weight1 * sum(x))) / n
		End Function

		''' <summary>
		''' This returns the minimized loss values for a given vector.
		''' It is assumed that  the x, y pairs are at
		''' vector[i], vector[i+1]
		''' </summary>
		''' <param name="vector"> the vector of numbers to getFromOrigin the weights for </param>
		''' <returns> a double array with w_0 and w_1 are the associated indices. </returns>
		Public Shared Function weightsFor(ByVal vector() As Double) As Double()

			' split coordinate system 
			Dim coords As IList(Of Double()) = coordSplit(vector)
			' x vals 
			Dim x() As Double = coords(0)
			' y vals 
			Dim y() As Double = coords(1)


			Dim meanX As Double = sum(x) / x.Length
			Dim meanY As Double = sum(y) / y.Length

			Dim sumOfMeanDifferences As Double = MathUtils.sumOfMeanDifferences(x, y)
			Dim xDifferenceOfMean As Double = sumOfMeanDifferencesOnePoint(x)

			Dim w_1 As Double = sumOfMeanDifferences / xDifferenceOfMean

			Dim w_0 As Double = meanY - (w_1) * meanX


			Dim ret(vector.Length - 1) As Double
			ret(0) = w_0
			ret(1) = w_1

			Return ret
		End Function 'end weightsFor

		Public Shared Function errorFor(ByVal actual As Double, ByVal prediction As Double) As Double
			Return actual - prediction
		End Function

		''' <summary>
		''' Used for calculating top part of simple regression for
		''' beta 1
		''' </summary>
		''' <param name="vector">  the x coordinates </param>
		''' <param name="vector2"> the y coordinates </param>
		''' <returns> the sum of mean differences for the input vectors </returns>
		Public Shared Function sumOfMeanDifferences(ByVal vector() As Double, ByVal vector2() As Double) As Double
			Dim mean As Double = sum(vector) / vector.Length
			Dim mean2 As Double = sum(vector2) / vector2.Length
			Dim ret As Double = 0
			For i As Integer = 0 To vector.Length - 1
				Dim vec1Diff As Double = vector(i) - mean
				Dim vec2Diff As Double = vector2(i) - mean2
				ret += vec1Diff * vec2Diff
			Next i
			Return ret
		End Function 'end sumOfMeanDifferences

		''' <summary>
		''' Used for calculating top part of simple regression for
		''' beta 1
		''' </summary>
		''' <param name="vector"> the x coordinates </param>
		''' <returns> the sum of mean differences for the input vectors </returns>
		Public Shared Function sumOfMeanDifferencesOnePoint(ByVal vector() As Double) As Double
			Dim mean As Double = sum(vector) / vector.Length
			Dim ret As Double = 0
			For i As Integer = 0 To vector.Length - 1
				Dim vec1Diff As Double = Math.Pow(vector(i) - mean, 2)
				ret += vec1Diff
			Next i
			Return ret
		End Function 'end sumOfMeanDifferences

		''' <summary>
		''' This returns the product of all numbers in the given array.
		''' </summary>
		''' <param name="nums"> the numbers to multiply over </param>
		''' <returns> the product of all numbers in the array, or 0
		''' if the length is or or nums i null </returns>
		Public Shared Function times(ByVal nums() As Double) As Double
			If nums Is Nothing OrElse nums.Length = 0 Then
				Return 0
			End If
			Dim ret As Double = 1
			For i As Integer = 0 To nums.Length - 1
				ret *= nums(i)
			Next i
			Return ret
		End Function 'end times

		''' <summary>
		''' This returns the sum of products for the given
		''' numbers.
		''' </summary>
		''' <param name="nums"> the sum of products for the give numbers </param>
		''' <returns> the sum of products for the given numbers </returns>
		Public Shared Function sumOfProducts(ParamArray ByVal nums()() As Double) As Double
			If nums Is Nothing OrElse nums.Length < 1 Then
				Return 0
			End If
			Dim sum As Double = 0

			Dim i As Integer = 0
			Do While i < nums.Length
				' The ith column for all of the rows 
				Dim column() As Double = MathUtils.column(i, nums)
				sum += times(column)

				i += 1
			Loop
			Return sum
		End Function 'end sumOfProducts

		''' <summary>
		''' This returns the given column over an n arrays
		''' </summary>
		''' <param name="column"> the column to getFromOrigin values for </param>
		''' <param name="nums">   the arrays to extract values from </param>
		''' <returns> a double array containing all of the numbers in that column
		''' for all of the arrays. </returns>
		''' <exception cref="IllegalArgumentException"> if the index is < 0 </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private static double[] column(int column, double[]... nums) throws IllegalArgumentException
'JAVA TO VB CONVERTER NOTE: The parameter column was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
		Private Shared Function column(ByVal column_Conflict As Integer, ParamArray ByVal nums()() As Double) As Double()

			Dim ret(nums.Length - 1) As Double

			For i As Integer = 0 To nums.Length - 1
				Dim curr() As Double = nums(i)
				ret(i) = curr(column_Conflict)
			Next i
			Return ret
		End Function 'end column

		''' <summary>
		''' This returns the coordinate split in a list of coordinates
		''' such that the values for ret[0] are the x values
		''' and ret[1] are the y values
		''' </summary>
		''' <param name="vector"> the vector to split with x and y values/ </param>
		''' <returns> a coordinate split for the given vector of values.
		''' if null, is passed in null is returned </returns>
		Public Shared Function coordSplit(ByVal vector() As Double) As IList(Of Double())

			If vector Is Nothing Then
				Return Nothing
			End If
			Dim ret As IList(Of Double()) = New List(Of Double())()
			' x coordinates 
			Dim xVals((vector.Length \ 2) - 1) As Double
			' y coordinates 
			Dim yVals((vector.Length \ 2) - 1) As Double
			' current points 
			Dim xTracker As Integer = 0
			Dim yTracker As Integer = 0
			For i As Integer = 0 To vector.Length - 1
				'even value, x coordinate
				If i Mod 2 = 0 Then
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: xVals[xTracker++] = vector[i];
					xVals(xTracker) = vector(i)
						xTracker += 1
				'y coordinate
				Else
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: yVals[yTracker++] = vector[i];
					yVals(yTracker) = vector(i)
						yTracker += 1
				End If
			Next i
			ret.Add(xVals)
			ret.Add(yVals)

			Return ret
		End Function 'end coordSplit

		''' <summary>
		''' This will partition the given whole variable data applyTransformToDestination in to the specified chunk number.
		''' </summary>
		''' <param name="arr">   the data applyTransformToDestination to pass in </param>
		''' <param name="chunk"> the number to separate by </param>
		''' <returns> a partition data applyTransformToDestination relative to the passed in chunk number </returns>
		Public Shared Function partitionVariable(ByVal arr As IList(Of Double), ByVal chunk As Integer) As IList(Of IList(Of Double))
			Dim count As Integer = 0
			Dim ret As IList(Of IList(Of Double)) = New List(Of IList(Of Double))()


			Do While count < arr.Count

				Dim sublist As IList(Of Double) = arr.subList(count, count + chunk)
				count += chunk
				ret.Add(sublist)

			Loop
			'All data sets must be same size
			For Each lists As IList(Of Double) In ret
				If lists.Count < chunk Then
					ret.Remove(lists)
				End If
			Next lists
			Return ret
		End Function 'end partitionVariable

		''' <summary>
		''' This returns the coordinate split in a list of coordinates
		''' such that the values for ret[0] are the x values
		''' and ret[1] are the y values
		''' </summary>
		''' <param name="vector"> the vector to split with x and y values
		'''               Note that the list will be more stable due to the size operator.
		'''               The array version will have extraneous values if not monitored
		'''               properly. </param>
		''' <returns> a coordinate split for the given vector of values.
		''' if null, is passed in null is returned </returns>
		Public Shared Function coordSplit(ByVal vector As IList(Of Double)) As IList(Of Double())

			If vector Is Nothing Then
				Return Nothing
			End If
			Dim ret As IList(Of Double()) = New List(Of Double())()
			' x coordinates 
			Dim xVals((vector.Count \ 2) - 1) As Double
			' y coordinates 
			Dim yVals((vector.Count \ 2) - 1) As Double
			' current points 
			Dim xTracker As Integer = 0
			Dim yTracker As Integer = 0
			For i As Integer = 0 To vector.Count - 1
				'even value, x coordinate
				If i Mod 2 = 0 Then
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: xVals[xTracker++] = vector.get(i);
					xVals(xTracker) = vector(i)
						xTracker += 1
				'y coordinate
				Else
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: yVals[yTracker++] = vector.get(i);
					yVals(yTracker) = vector(i)
						yTracker += 1
				End If
			Next i
			ret.Add(xVals)
			ret.Add(yVals)

			Return ret
		End Function 'end coordSplit

		''' <summary>
		''' This returns the x values of the given vector.
		''' These are assumed to be the even values of the vector.
		''' </summary>
		''' <param name="vector"> the vector to getFromOrigin the values for </param>
		''' <returns> the x values of the given vector </returns>
		Public Shared Function xVals(ByVal vector() As Double) As Double()


			If vector Is Nothing Then
				Return Nothing
			End If
			Dim x((vector.Length \ 2) - 1) As Double
			Dim count As Integer = 0
			For i As Integer = 0 To vector.Length - 1
				If i Mod 2 <> 0 Then
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: x[count++] = vector[i];
					x(count) = vector(i)
						count += 1
				End If
			Next i
			Return x
		End Function 'end xVals

		''' <summary>
		''' This returns the odd indexed values for the given vector
		''' </summary>
		''' <param name="vector"> the odd indexed values of rht egiven vector </param>
		''' <returns> the y values of the given vector </returns>
		Public Shared Function yVals(ByVal vector() As Double) As Double()
			Dim y((vector.Length \ 2) - 1) As Double
			Dim count As Integer = 0
			For i As Integer = 0 To vector.Length - 1
				If i Mod 2 = 0 Then
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: y[count++] = vector[i];
					y(count) = vector(i)
						count += 1
				End If
			Next i
			Return y
		End Function 'end yVals

		''' <summary>
		''' This returns the sum of squares for the given vector.
		''' </summary>
		''' <param name="vector"> the vector to obtain the sum of squares for </param>
		''' <returns> the sum of squares for this vector </returns>
		Public Shared Function sumOfSquares(ByVal vector() As Double) As Double
			Dim ret As Double = 0
			For Each d As Double In vector
				ret += Math.Pow(d, 2)
			Next d
			Return ret
		End Function

		''' <summary>
		''' This returns the determination coefficient of two vectors given a length
		''' </summary>
		''' <param name="y1"> the first vector </param>
		''' <param name="y2"> the second vector </param>
		''' <param name="n">  the length of both vectors </param>
		''' <returns> the determination coefficient or r^2 </returns>
		Public Shared Function determinationCoefficient(ByVal y1() As Double, ByVal y2() As Double, ByVal n As Integer) As Double
			Return Math.Pow(correlation(y1, y2), 2)
		End Function

		''' <summary>
		''' Returns the logarithm of a for base 2.
		''' </summary>
		''' <param name="a"> a double </param>
		''' <returns> the logarithm for base 2 </returns>
		Public Shared Function log2(ByVal a As Double) As Double
			If a = 0 Then
				Return 0.0
			End If
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
			Return Math.Log(a) / log2_Conflict
		End Function

		''' <summary>
		''' This returns the root mean squared error of two data sets
		''' </summary>
		''' <param name="real">      the realComponent values </param>
		''' <param name="predicted"> the predicted values </param>
		''' <returns> the root means squared error for two data sets </returns>
		Public Shared Function rootMeansSquaredError(ByVal real() As Double, ByVal predicted() As Double) As Double
			Dim ret As Double = 1 \ real.Length
			For i As Integer = 0 To real.Length - 1
				ret += Math.Pow((real(i) - predicted(i)), 2)
			Next i
			Return Math.Sqrt(ret)
		End Function 'end rootMeansSquaredError

		''' <summary>
		''' This returns the entropy (information gain, or uncertainty of a random variable): -sum(x*log(x))
		''' </summary>
		''' <param name="vector"> the vector of values to getFromOrigin the entropy for </param>
		''' <returns> the entropy of the given vector </returns>
		Public Shared Function entropy(ByVal vector() As Double) As Double
			If vector Is Nothing OrElse vector.Length = 0 Then
				Return 0
			Else
				Dim ret As Double = 0
				For Each d As Double In vector
					ret += d * Math.Log(d)
				Next d
				Return -ret

			End If
		End Function 'end entropy

		''' <summary>
		''' This returns the kronecker delta of two doubles.
		''' </summary>
		''' <param name="i"> the first number to compare </param>
		''' <param name="j"> the second number to compare </param>
		''' <returns> 1 if they are equal, 0 otherwise </returns>
		Public Shared Function kroneckerDelta(ByVal i As Double, ByVal j As Double) As Integer
			Return If(i = j, 1, 0)
		End Function

		''' <summary>
		''' This calculates the adjusted r^2 including degrees of freedom.
		''' Also known as calculating "strength" of a regression
		''' </summary>
		''' <param name="rSquared">      the r squared value to calculate </param>
		''' <param name="numRegressors"> number of variables </param>
		''' <param name="numDataPoints"> size of the data applyTransformToDestination </param>
		''' <returns> an adjusted r^2 for degrees of freedom </returns>
		Public Shared Function adjustedrSquared(ByVal rSquared As Double, ByVal numRegressors As Integer, ByVal numDataPoints As Integer) As Double
			Dim divide As Double = (numDataPoints - 1) \ (numDataPoints - numRegressors - 1)
			Dim rSquaredDiff As Double = 1 - rSquared
			Return 1 - (rSquaredDiff * divide)
		End Function


		Public Shared Function normalizeToOne(ByVal doubles() As Double) As Double()
			normalize(doubles, sum(doubles))
			Return doubles
		End Function

		Public Shared Function min(ByVal doubles() As Double) As Double
			Dim ret As Double = doubles(0)
			For Each d As Double In doubles
				If d < ret Then
					ret = d
				End If
			Next d
			Return ret
		End Function

		Public Shared Function max(ByVal doubles() As Double) As Double
			Dim ret As Double = doubles(0)
			For Each d As Double In doubles
				If d > ret Then
					ret = d
				End If
			Next d
			Return ret
		End Function

		''' <summary>
		''' Normalizes the doubles in the array using the given value.
		''' </summary>
		''' <param name="doubles"> the array of double </param>
		''' <param name="sum">     the value by which the doubles are to be normalized </param>
		''' <exception cref="IllegalArgumentException"> if sum is zero or NaN </exception>
		Public Shared Sub normalize(ByVal doubles() As Double, ByVal sum As Double)

			If Double.IsNaN(sum) Then
				Throw New System.ArgumentException("Can't normalize array. Sum is NaN.")
			End If
			If sum = 0 Then
				' Maybe this should just be a return.
				Throw New System.ArgumentException("Can't normalize array. Sum is zero.")
			End If
			For i As Integer = 0 To doubles.Length - 1
				doubles(i) /= sum
			Next i
		End Sub 'end normalize

		''' <summary>
		''' Converts an array containing the natural logarithms of
		''' probabilities stored in a vector back into probabilities.
		''' The probabilities are assumed to sum to one.
		''' </summary>
		''' <param name="a"> an array holding the natural logarithms of the probabilities </param>
		''' <returns> the converted array </returns>
		Public Shared Function logs2probs(ByVal a() As Double) As Double()

			Dim max As Double = a(maxIndex(a))
			Dim sum As Double = 0.0

			Dim result(a.Length - 1) As Double
			For i As Integer = 0 To a.Length - 1
				result(i) = Math.Exp(a(i) - max)
				sum += result(i)
			Next i

			normalize(result, sum)

			Return result
		End Function 'end logs2probs

		''' <summary>
		''' This returns the entropy for a given vector of probabilities.
		''' </summary>
		''' <param name="probabilities"> the probabilities to getFromOrigin the entropy for </param>
		''' <returns> the entropy of the given probabilities. </returns>
		Public Shared Function information(ByVal probabilities() As Double) As Double
			Dim total As Double = 0.0
			For Each d As Double In probabilities
				total += (-1.0 * log2(d) * d)
			Next d
			Return total
		End Function 'end information

		''' <summary>
		''' Returns index of maximum element in a given
		''' array of doubles. First maximum is returned.
		''' </summary>
		''' <param name="doubles"> the array of doubles </param>
		''' <returns> the index of the maximum element </returns>
		Public Shared Function maxIndex(ByVal doubles() As Double) As Integer

			Dim maximum As Double = 0
'JAVA TO VB CONVERTER NOTE: The local variable maxIndex was renamed since Visual Basic will not allow local variables with the same name as their enclosing function or property:
			Dim maxIndex_Conflict As Integer = 0

			For i As Integer = 0 To doubles.Length - 1
				If (i = 0) OrElse (doubles(i) > maximum) Then
					maxIndex_Conflict = i
					maximum = doubles(i)
				End If
			Next i

			Return maxIndex_Conflict
		End Function 'end maxIndex

		''' <summary>
		''' This will return the factorial of the given number n.
		''' </summary>
		''' <param name="n"> the number to getFromOrigin the factorial for </param>
		''' <returns> the factorial for this number </returns>
		Public Shared Function factorial(ByVal n As Double) As Double
			If n = 1 OrElse n = 0 Then
				Return 1
			End If
			Dim i As Double = n
			Do While i > 0
				i -= 1
				n *= (If(i > 0, i, 1))
			Loop
			Return n
		End Function 'end factorial

		''' <summary>
		''' Returns the log-odds for a given probability.
		''' </summary>
		''' <param name="prob"> the probability </param>
		''' <returns> the log-odds after the probability has been mapped to
		''' [Utils.SMALL, 1-Utils.SMALL] </returns>
		Public Shared Function probToLogOdds(ByVal prob As Double) As Double

			If gr(prob, 1) OrElse (sm(prob, 0)) Then
				Throw New System.ArgumentException("probToLogOdds: probability must " & "be in [0,1] " & prob)
			End If
			Dim p As Double = SMALL + (1.0 - 2 * SMALL) * prob
			Return Math.Log(p / (1 - p))
		End Function

		''' <summary>
		''' Rounds a double to the next nearest integer value. The JDK version
		''' of it doesn't work properly.
		''' </summary>
		''' <param name="value"> the double value </param>
		''' <returns> the resulting integer value </returns>
		Public Shared Function round(ByVal value As Double) As Integer

			Dim roundedValue As Integer = If(value > 0, CInt(Math.Truncate(value + 0.5)), -CInt(Math.Truncate(Math.Abs(value) + 0.5)))

			Return roundedValue
		End Function 'end round

		''' <summary>
		''' This returns the permutation of n choose r.
		''' </summary>
		''' <param name="n"> the n to choose </param>
		''' <param name="r"> the number of elements to choose </param>
		''' <returns> the permutation of these numbers </returns>
		Public Shared Function permutation(ByVal n As Double, ByVal r As Double) As Double
			Dim nFac As Double = MathUtils.factorial(n)
			Dim nMinusRFac As Double = MathUtils.factorial((n - r))
			Return nFac / nMinusRFac
		End Function 'end permutation

		''' <summary>
		''' This returns the combination of n choose r
		''' </summary>
		''' <param name="n"> the number of elements overall </param>
		''' <param name="r"> the number of elements to choose </param>
		''' <returns> the amount of possible combinations for this applyTransformToDestination of elements </returns>
		Public Shared Function combination(ByVal n As Double, ByVal r As Double) As Double
			Dim nFac As Double = MathUtils.factorial(n)
			Dim rFac As Double = MathUtils.factorial(r)
			Dim nMinusRFac As Double = MathUtils.factorial((n - r))

			Return nFac / (rFac * nMinusRFac)
		End Function 'end combination

		''' <summary>
		''' sqrt(a^2 + b^2) without under/overflow.
		''' </summary>
		Public Shared Function hypotenuse(ByVal a As Double, ByVal b As Double) As Double
			Dim r As Double
			If Math.Abs(a) > Math.Abs(b) Then
				r = b / a
				r = Math.Abs(a) * Math.Sqrt(1 + r * r)
			ElseIf b <> 0 Then
				r = a / b
				r = Math.Abs(b) * Math.Sqrt(1 + r * r)
			Else
				r = 0.0
			End If
			Return r
		End Function 'end hypotenuse

		''' <summary>
		''' Rounds a double to the next nearest integer value in a probabilistic
		''' fashion (e.g. 0.8 has a 20% chance of being rounded down to 0 and a
		''' 80% chance of being rounded up to 1). In the limit, the average of
		''' the rounded numbers generated by this procedure should converge to
		''' the original double.
		''' </summary>
		''' <param name="value"> the double value </param>
		''' <param name="rand">  the random number generator </param>
		''' <returns> the resulting integer value </returns>
		Public Shared Function probRound(ByVal value As Double, ByVal rand As Random) As Integer

			If value >= 0 Then
				Dim lower As Double = Math.Floor(value)
				Dim prob As Double = value - lower
				If rand.NextDouble() < prob Then
					Return CInt(Math.Truncate(lower)) + 1
				Else
					Return CInt(Math.Truncate(lower))
				End If
			Else
				Dim lower As Double = Math.Floor(Math.Abs(value))
				Dim prob As Double = Math.Abs(value) - lower
				If rand.NextDouble() < prob Then
					Return -(CInt(Math.Truncate(lower)) + 1)
				Else
					Return -CInt(Math.Truncate(lower))
				End If
			End If
		End Function 'end probRound

		''' <summary>
		''' Rounds a double to the given number of decimal places.
		''' </summary>
		''' <param name="value">             the double value </param>
		''' <param name="afterDecimalPoint"> the number of digits after the decimal point </param>
		''' <returns> the double rounded to the given precision </returns>
		Public Shared Function roundDouble(ByVal value As Double, ByVal afterDecimalPoint As Integer) As Double

			Dim mask As Double = Math.Pow(10.0, CDbl(afterDecimalPoint))

			Return CDbl(CLng(Math.Round(value * mask, MidpointRounding.AwayFromZero))) / mask
		End Function 'end roundDouble

		''' <summary>
		''' Rounds a double to the given number of decimal places.
		''' </summary>
		''' <param name="value">             the double value </param>
		''' <param name="afterDecimalPoint"> the number of digits after the decimal point </param>
		''' <returns> the double rounded to the given precision </returns>
		Public Shared Function roundFloat(ByVal value As Single, ByVal afterDecimalPoint As Integer) As Single

			Dim mask As Single = CSng(Math.Pow(10, CSng(afterDecimalPoint)))

			Return CSng(CInt(Math.Round(value * mask, MidpointRounding.AwayFromZero))) / mask
		End Function 'end roundDouble

		''' <summary>
		''' This will return the bernoulli trial for the given event.
		''' A bernoulli trial is a mechanism for detecting the probability
		''' of a given event occurring k times in n independent trials
		''' </summary>
		''' <param name="n">           the number of trials </param>
		''' <param name="k">           the number of times the target event occurs </param>
		''' <param name="successProb"> the probability of the event happening </param>
		''' <returns> the probability of the given event occurring k times. </returns>
		Public Shared Function bernoullis(ByVal n As Double, ByVal k As Double, ByVal successProb As Double) As Double

			Dim combo As Double = MathUtils.combination(n, k)
			Dim p As Double = successProb
			Dim q As Double = 1 - successProb
			Return combo * Math.Pow(p, k) * Math.Pow(q, n - k)
		End Function 'end bernoullis

		''' <summary>
		''' Tests if a is smaller than b.
		''' </summary>
		''' <param name="a"> a double </param>
		''' <param name="b"> a double </param>
		Public Shared Function sm(ByVal a As Double, ByVal b As Double) As Boolean

			Return (b - a > SMALL)
		End Function

		''' <summary>
		''' Tests if a is greater than b.
		''' </summary>
		''' <param name="a"> a double </param>
		''' <param name="b"> a double </param>
		Public Shared Function gr(ByVal a As Double, ByVal b As Double) As Boolean

			Return (a - b > SMALL)
		End Function

		''' <summary>
		''' This will take a given string and separator and convert it to an equivalent
		''' double array.
		''' </summary>
		''' <param name="data">      the data to separate </param>
		''' <param name="separator"> the separator to use </param>
		''' <returns> the new double array based on the given data </returns>
		Public Shared Function fromString(ByVal data As String, ByVal separator As String) As Double()
			Dim split() As String = data.Split(separator, True)
			Dim ret(split.Length - 1) As Double
			For i As Integer = 0 To split.Length - 1
				ret(i) = Double.Parse(split(i))
			Next i
			Return ret
		End Function 'end fromString

		''' <summary>
		''' Computes the mean for an array of doubles.
		''' </summary>
		''' <param name="vector"> the array </param>
		''' <returns> the mean </returns>
		Public Shared Function mean(ByVal vector() As Double) As Double

			Dim sum As Double = 0

			If vector.Length = 0 Then
				Return 0
			End If
			For i As Integer = 0 To vector.Length - 1
				sum += vector(i)
			Next i
			Return sum / CDbl(vector.Length)
		End Function 'end mean

		''' <summary>
		''' This will convert the given binary string to a decimal based
		''' integer
		''' </summary>
		''' <param name="binary"> the binary string to convert </param>
		''' <returns> an equivalent base 10 number </returns>
		Public Shared Function toDecimal(ByVal binary As String) As Integer
			Dim num As Long = Long.Parse(binary)
			Dim [rem] As Long
			' Use the remainder method to ensure validity 
			Do While num > 0
				[rem] = num Mod 10
				num = num \ 10
				If [rem] <> 0 AndAlso [rem] <> 1 Then
					Console.WriteLine("This is not a binary number.")
					Console.WriteLine("Please try once again.")
					Return -1
				End If
			Loop
			Dim i As Integer = Convert.ToInt32(binary, 2)
			Return i
		End Function 'end toDecimal

		''' <summary>
		''' This will translate a vector in to an equivalent integer
		''' </summary>
		''' <param name="vector"> the vector to translate </param>
		''' <returns> a z value such that the value is the interleaved lsd to msd for each
		''' double in the vector </returns>
		Public Shared Function distanceFinderZValue(ByVal vector() As Double) As Integer
			Dim binaryBuffer As New StringBuilder()
			Dim binaryReps As IList(Of String) = New List(Of String)(vector.Length)
			For i As Integer = 0 To vector.Length - 1
				Dim d As Double = vector(i)
				Dim j As Integer = CInt(Math.Truncate(d))
				Dim binary As String = Integer.toBinaryString(j)
				binaryReps.Add(binary)
			Next i
			'append from left to right, the least to the most significant bit
			'till all strings are empty
			Do While binaryReps.Count > 0
				Dim j As Integer = 0
				Do While j < binaryReps.Count
					Dim curr As String = binaryReps(j)
					If curr.Length > 0 Then
						Dim first As Char = curr.Chars(0)
						binaryBuffer.Append(first)
						curr = curr.Substring(1)
						binaryReps(j) = curr
					Else
						binaryReps.RemoveAt(j)
					End If
					j += 1
				Loop
			Loop
			Return Convert.ToInt32(binaryBuffer.ToString(), 2)

		End Function 'end distanceFinderZValue

		''' <summary>
		''' This returns the euclidean distance of two vectors
		''' sum(i=1,n)   (q_i - p_i)^2
		''' </summary>
		''' <param name="p"> the first vector </param>
		''' <param name="q"> the second vector </param>
		''' <returns> the euclidean distance between two vectors </returns>
		Public Shared Function euclideanDistance(ByVal p() As Double, ByVal q() As Double) As Double

			Dim ret As Double = 0
			For i As Integer = 0 To p.Length - 1
				Dim diff As Double = (q(i) - p(i))
				Dim sq As Double = Math.Pow(diff, 2)
				ret += sq
			Next i
			Return ret

		End Function 'end euclideanDistance

		''' <summary>
		''' This returns the euclidean distance of two vectors
		''' sum(i=1,n)   (q_i - p_i)^2
		''' </summary>
		''' <param name="p"> the first vector </param>
		''' <param name="q"> the second vector </param>
		''' <returns> the euclidean distance between two vectors </returns>
		Public Shared Function euclideanDistance(ByVal p() As Single, ByVal q() As Single) As Double

			Dim ret As Double = 0
			For i As Integer = 0 To p.Length - 1
				Dim diff As Double = (q(i) - p(i))
				Dim sq As Double = Math.Pow(diff, 2)
				ret += sq
			Next i
			Return ret

		End Function 'end euclideanDistance

		''' <summary>
		''' This will generate a series of uniformally distributed
		''' numbers between l times
		''' </summary>
		''' <param name="l"> the number of numbers to generate </param>
		''' <returns> l uniformally generated numbers </returns>
		Public Shared Function generateUniform(ByVal l As Integer) As Double()
			Dim ret(l - 1) As Double
			Dim rgen As New Random()
			For i As Integer = 0 To l - 1
				ret(i) = rgen.NextDouble()
			Next i
			Return ret
		End Function 'end generateUniform

		''' <summary>
		''' This will calculate the Manhattan distance between two sets of points.
		''' The Manhattan distance is equivalent to:
		''' 1_sum_n |p_i - q_i|
		''' </summary>
		''' <param name="p"> the first point vector </param>
		''' <param name="q"> the second point vector </param>
		''' <returns> the Manhattan distance between two object </returns>
		Public Shared Function manhattanDistance(ByVal p() As Double, ByVal q() As Double) As Double

			Dim ret As Double = 0
			For i As Integer = 0 To p.Length - 1
				Dim difference As Double = p(i) - q(i)
				ret += Math.Abs(difference)
			Next i
			Return ret
		End Function 'end manhattanDistance

		Public Shared Function sampleDoublesInInterval(ByVal doubles()() As Double, ByVal l As Integer) As Double()
			Dim sample(l - 1) As Double
			For i As Integer = 0 To l - 1
				Dim rand1 As Integer = randomNumberBetween(0, doubles.Length - 1)
				Dim rand2 As Integer = randomNumberBetween(0, doubles(i).Length)
				sample(i) = doubles(rand1)(rand2)
			Next i

			Return sample
		End Function

		''' <summary>
		''' Generates a random integer between the specified numbers
		''' </summary>
		''' <param name="begin"> the begin of the interval </param>
		''' <param name="end">   the end of the interval </param>
		''' <param name="anchor"> the base number (assuming to be generated from an external rng) </param>
		''' <returns> an int between begin and end </returns>
		Public Shared Function randomNumberBetween(ByVal begin As Double, ByVal [end] As Double, ByVal anchor As Double) As Integer
			If begin > [end] Then
				Throw New System.ArgumentException("Begin must not be less than end")
			End If
			Return CInt(Math.Truncate(begin)) + CInt(Math.Truncate(anchor * (([end] - begin) + 1)))
		End Function


		''' <summary>
		''' Generates a random integer between the specified numbers
		''' </summary>
		''' <param name="begin"> the begin of the interval </param>
		''' <param name="end">   the end of the interval </param>
		''' <returns> an int between begin and end </returns>
		Public Shared Function randomNumberBetween(ByVal begin As Double, ByVal [end] As Double) As Integer
			If begin > [end] Then
				Throw New System.ArgumentException("Begin must not be less than end")
			End If
			Return CInt(Math.Truncate(begin)) + CInt(Math.Truncate(MathHelper.NextDouble * (([end] - begin) + 1)))
		End Function

		''' <summary>
		''' Generates a random integer between the specified numbers
		''' </summary>
		''' <param name="begin"> the begin of the interval </param>
		''' <param name="end">   the end of the interval </param>
		''' <returns> an int between begin and end </returns>
		Public Shared Function randomNumberBetween(ByVal begin As Double, ByVal [end] As Double, ByVal rng As RandomGenerator) As Integer
			If begin > [end] Then
				Throw New System.ArgumentException("Begin must not be less than end")
			End If
			Return CInt(Math.Truncate(begin)) + CInt(Math.Truncate(rng.nextDouble() * (([end] - begin) + 1)))
		End Function

		Public Shared Function randomFloatBetween(ByVal begin As Single, ByVal [end] As Single) As Single
			Dim rand As Single = CSng(MathHelper.NextDouble)
			Return begin + (rand * ([end] - begin))
		End Function

		Public Shared Function randomDoubleBetween(ByVal begin As Double, ByVal [end] As Double) As Double
			Return begin + (MathHelper.NextDouble * ([end] - begin))
		End Function

		''' <summary>
		''' This returns the slope of the given points.
		''' </summary>
		''' <param name="x1"> the first x to use </param>
		''' <param name="x2"> the end x to use </param>
		''' <param name="y1"> the begin y to use </param>
		''' <param name="y2"> the end y to use </param>
		''' <returns> the slope of the given points </returns>
		Public Overridable Function slope(ByVal x1 As Double, ByVal x2 As Double, ByVal y1 As Double, ByVal y2 As Double) As Double
			Return (y2 - y1) / (x2 - x1)
		End Function 'end slope

		''' <summary>
		''' Shuffle the array elements using the specified RNG seed.
		''' Uses Fisher Yates shuffle internally: <a href="https://en.wikipedia.org/wiki/Fisher%E2%80%93Yates_shuffle#The_modern_algorithm">
		''' https://en.wikipedia.org/wiki/Fisher%E2%80%93Yates_shuffle#The_modern_algorithm</a>
		''' </summary>
		''' <param name="array">   Array to shuffle </param>
		''' <param name="rngSeed"> RNG seed to use for shuffling </param>
		Public Shared Sub shuffleArray(ByVal array() As Integer, ByVal rngSeed As Long)
			shuffleArray(array, New Random(rngSeed))
		End Sub

		''' <summary>
		''' Shuffle the array elements using the specified Random instance
		''' Uses Fisher Yates shuffle internally: <a href="https://en.wikipedia.org/wiki/Fisher%E2%80%93Yates_shuffle#The_modern_algorithm">
		''' https://en.wikipedia.org/wiki/Fisher%E2%80%93Yates_shuffle#The_modern_algorithm</a>
		''' </summary>
		''' <param name="array"> Array to shuffle </param>
		''' <param name="rng">   Random instance to use for shuffling </param>
		Public Shared Sub shuffleArray(ByVal array() As Integer, ByVal rng As Random)
			shuffleArraySubset(array, array.Length, rng)
		End Sub

		''' <summary>
		''' Shuffle the first N elements of the array using the specified Random instance.<br>
		''' If shuffleFirst < array.length, only the elements 0 to shuffleFirst-1 are modified; values at indices shuffleFirst to
		''' array.length-1 are not changed.
		''' Uses Fisher Yates shuffle internally: <a href="https://en.wikipedia.org/wiki/Fisher%E2%80%93Yates_shuffle#The_modern_algorithm">
		''' https://en.wikipedia.org/wiki/Fisher%E2%80%93Yates_shuffle#The_modern_algorithm</a>
		''' </summary>
		''' <param name="array"> Array to shuffle first N elements of
		''' </param>
		''' <param name="rng">   Random instance to use for shuffling </param>
		Public Shared Sub shuffleArraySubset(ByVal array() As Integer, ByVal shuffleFirst As Integer, ByVal rng As Random)
			For i As Integer = shuffleFirst-1 To 1 Step -1
				Dim j As Integer = rng.Next(i + 1)
				Dim temp As Integer = array(j)
				array(j) = array(i)
				array(i) = temp
			Next i
		End Sub

		''' <summary>
		''' hashCode method, taken from Java 1.8 Double.hashCode(double) method
		''' </summary>
		''' <param name="value"> Double value to hash </param>
		''' <returns> Hash code for the double value </returns>
		Public Shared Function hashCode(ByVal value As Double) As Integer
			Dim bits As Long = System.BitConverter.DoubleToInt64Bits(value)
			Return CInt(bits Xor (CLng(CULng(bits) >> 32)))
		End Function
	End Class

End Namespace