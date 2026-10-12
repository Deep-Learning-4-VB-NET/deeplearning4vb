Imports System
Imports System.Collections.Generic
Imports NonNull = lombok.NonNull
Imports val = lombok.val
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports FirstIndex = org.nd4j.linalg.api.ops.impl.indexaccum.FirstIndex
Imports LastIndex = org.nd4j.linalg.api.ops.impl.indexaccum.LastIndex
Imports MatchCondition = org.nd4j.linalg.api.ops.impl.reduce.longer.MatchCondition
Imports CompareAndReplace = org.nd4j.linalg.api.ops.impl.transforms.comparison.CompareAndReplace
Imports CompareAndSet = org.nd4j.linalg.api.ops.impl.transforms.comparison.CompareAndSet
Imports Choose = org.nd4j.linalg.api.ops.impl.transforms.custom.Choose
Imports Shape = org.nd4j.linalg.api.shape.Shape
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports BaseCondition = org.nd4j.linalg.indexing.conditions.BaseCondition
Imports Condition = org.nd4j.linalg.indexing.conditions.Condition

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

Namespace org.nd4j.linalg.indexing


	''' <summary>
	''' Boolean indexing
	''' 
	''' @author Adam Gibson
	''' </summary>
	Public Class BooleanIndexing

		''' <summary>
		''' And over the whole ndarray given some condition
		''' </summary>
		''' <param name="n">    the ndarray to test </param>
		''' <param name="cond"> the condition to test against </param>
		''' <returns> true if all of the elements meet the specified
		''' condition false otherwise </returns>
'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public static boolean and(final org.nd4j.linalg.api.ndarray.INDArray n, final org.nd4j.linalg.indexing.conditions.Condition cond)
		Public Shared Function [and](ByVal n As INDArray, ByVal cond As Condition) As Boolean
			If TypeOf cond Is BaseCondition Then
				Dim val As Long = CLng(Math.Truncate(Nd4j.Executioner.exec(New MatchCondition(n, cond)).getDouble(0)))

				If val = n.length() Then
					Return True
				Else
					Return False
				End If

			Else
				Throw New Exception("Can only execute BaseCondition conditions using this method")
			End If
		End Function

		''' <summary>
		''' And over the whole ndarray given some condition, with respect to dimensions
		''' </summary>
		''' <param name="n">    the ndarray to test </param>
		''' <param name="condition"> the condition to test against </param>
		''' <returns> true if all of the elements meet the specified
		''' condition false otherwise </returns>
'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public static boolean[] and(final org.nd4j.linalg.api.ndarray.INDArray n, final org.nd4j.linalg.indexing.conditions.Condition condition, int... dimension)
		Public Shared Function [and](ByVal n As INDArray, ByVal condition As Condition, ParamArray ByVal dimension() As Integer) As Boolean()
			If Not (TypeOf condition Is BaseCondition) Then
				Throw New System.NotSupportedException("Only static Conditions are supported")
			End If

			Dim op As New MatchCondition(n, condition, dimension)
			Dim arr As INDArray = Nd4j.Executioner.exec(op)
			Dim result(CInt(arr.length()) - 1) As Boolean

			Dim tadLength As Long = Shape.getTADLength(n.shape(), dimension)

			For i As Integer = 0 To arr.length() - 1
				If arr.getDouble(i) = tadLength Then
					result(i) = True
				Else
					result(i) = False
				End If
			Next i

			Return result
		End Function


		''' <summary>
		''' Or over the whole ndarray given some condition, with respect to dimensions
		''' </summary>
		''' <param name="n">    the ndarray to test </param>
		''' <param name="condition"> the condition to test against </param>
		''' <returns> true if all of the elements meet the specified
		''' condition false otherwise </returns>
'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public static boolean[] or(final org.nd4j.linalg.api.ndarray.INDArray n, final org.nd4j.linalg.indexing.conditions.Condition condition, int... dimension)
		Public Shared Function [or](ByVal n As INDArray, ByVal condition As Condition, ParamArray ByVal dimension() As Integer) As Boolean()
			If Not (TypeOf condition Is BaseCondition) Then
				Throw New System.NotSupportedException("Only static Conditions are supported")
			End If

			Dim op As New MatchCondition(n, condition, dimension)
			Dim arr As INDArray = Nd4j.Executioner.exec(op)

			Dim result(CInt(arr.length()) - 1) As Boolean

			For i As Integer = 0 To arr.length() - 1
				If arr.getDouble(i) > 0 Then
					result(i) = True
				Else
					result(i) = False
				End If
			Next i

			Return result
		End Function

		''' <summary>
		''' Or over the whole ndarray given some condition
		''' </summary>
		''' <param name="n"> </param>
		''' <param name="cond">
		''' @return </param>
'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public static boolean or(final org.nd4j.linalg.api.ndarray.INDArray n, final org.nd4j.linalg.indexing.conditions.Condition cond)
		Public Shared Function [or](ByVal n As INDArray, ByVal cond As Condition) As Boolean
			If TypeOf cond Is BaseCondition Then
				Dim val As Long = CLng(Math.Truncate(Nd4j.Executioner.exec(New MatchCondition(n, cond)).getDouble(0)))

				If val > 0 Then
					Return True
				Else
					Return False
				End If

			Else
				Throw New Exception("Can only execute BaseCondition conditions using this method")
			End If
		End Function

		''' <summary>
		''' This method does element-wise comparison
		''' for 2 equal-sized matrices, for each element that matches Condition.
		''' To is the array to apply the indexing to
		''' from is a condition mask array (0 or 1).
		''' This would come from the output of a bit masking method like:
		''' <seealso cref="INDArray.gt(Number)"/>,<seealso cref="INDArray.gte(Number)"/>,
		''' <seealso cref="INDArray.lt(Number)"/>,..
		''' </summary>
		''' <param name="to"> the array to apply the condition to </param>
		''' <param name="from"> the mask array </param>
		''' <param name="condition"> the condition to apply </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static void assignIf(@NonNull INDArray to, @NonNull INDArray from, @NonNull Condition condition)
		Public Shared Sub assignIf(ByVal [to] As INDArray, ByVal from As INDArray, ByVal condition As Condition)
			If Not (TypeOf condition Is BaseCondition) Then
				Throw New System.NotSupportedException("Only static Conditions are supported")
			End If

			If [to].length() <> from.length() Then
				Throw New System.InvalidOperationException("Mis matched length for to and from")
			End If

			Nd4j.Executioner.exec(New CompareAndSet([to], from, [to], condition))
		End Sub


		''' <summary>
		''' This method does element-wise comparison for 2 equal-sized matrices, for each element that matches Condition
		''' </summary>
		''' <param name="to"> </param>
		''' <param name="from"> </param>
		''' <param name="condition"> </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static void replaceWhere(@NonNull INDArray to, @NonNull INDArray from, @NonNull Condition condition)
		Public Shared Sub replaceWhere(ByVal [to] As INDArray, ByVal from As INDArray, ByVal condition As Condition)
			If Not (TypeOf condition Is BaseCondition) Then
				Throw New System.NotSupportedException("Only static Conditions are supported")
			End If

			If [to].length() <> from.length() Then
				Throw New System.InvalidOperationException("Mis matched length for to and from")
			End If

			Nd4j.Executioner.exec(New CompareAndReplace([to], from, [to], condition))
		End Sub

		''' <summary>
		''' Choose from the inputs based on the given condition.
		''' This returns a row vector of all elements fulfilling the
		''' condition listed within the array for input </summary>
		''' <param name="input"> the input to filter </param>
		''' <param name="condition"> the condition to filter based on </param>
		''' <returns> a row vector of the input elements that are true
		''' ffor the given conditions </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static org.nd4j.linalg.api.ndarray.INDArray chooseFrom(@NonNull INDArray[] input,@NonNull Condition condition)
		Public Shared Function chooseFrom(ByVal input() As INDArray, ByVal condition As Condition) As INDArray
			Dim choose As val = New Choose(input,condition)
			Dim outputs As val = Nd4j.exec(choose)
			Dim secondOutput As Integer = outputs(1).getInt(0)
			If secondOutput < 1 Then
				Return Nothing
			End If

			Return choose.getOutputArgument(0)
		End Function

		''' <summary>
		''' A minor shortcut for applying a bitmask to
		''' a matrix </summary>
		''' <param name="arr"> The array to apply the mask to </param>
		''' <param name="mask"> the mask to apply </param>
		''' <returns> the array with the mask applied </returns>
		Public Shared Function applyMask(ByVal arr As INDArray, ByVal mask As INDArray) As INDArray
			Return arr.mul(mask)
		End Function

		''' <summary>
		''' A minor shortcut for applying a bitmask to
		''' a matrix </summary>
		''' <param name="arr"> The array to apply the mask to </param>
		''' <param name="mask"> the mask to apply </param>
		''' <returns> the array with the mask applied </returns>
		Public Shared Function applyMaskInPlace(ByVal arr As INDArray, ByVal mask As INDArray) As INDArray
			Return arr.muli(mask)
		End Function



		''' <summary>
		''' Choose from the inputs based on the given condition.
		''' This returns a row vector of all elements fulfilling the
		''' condition listed within the array for input.
		''' The double and integer arguments are only relevant
		''' for scalar operations (like when you have a scalar
		''' you are trying to compare each element in your input against)
		''' </summary>
		''' <param name="input"> the input to filter </param>
		''' <param name="tArgs"> the double args </param>
		''' <param name="iArgs"> the integer args </param>
		''' <param name="condition"> the condition to filter based on </param>
		''' <returns> a row vector of the input elements that are true
		''' ffor the given conditions </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static org.nd4j.linalg.api.ndarray.INDArray chooseFrom(@NonNull INDArray[] input, @NonNull List<Double> tArgs, @NonNull List<Integer> iArgs, @NonNull Condition condition)
		Public Shared Function chooseFrom(ByVal input() As INDArray, ByVal tArgs As IList(Of Double), ByVal iArgs As IList(Of Integer), ByVal condition As Condition) As INDArray
			Dim choose As New Choose(input,iArgs,tArgs,condition)
			Nd4j.Executioner.execAndReturn(choose)
			Dim secondOutput As Integer = choose.getOutputArgument(1).getInt(0)
			If secondOutput < 1 Then
				Return Nothing
			End If

			Dim ret As INDArray = choose.getOutputArgument(0).get(NDArrayIndex.interval(0,secondOutput))
			ret = ret.reshape(ChrW(ret.length()))
			Return ret
		End Function

		''' <summary>
		''' This method does element-wise assessing for 2 equal-sized matrices, for each element that matches Condition
		''' </summary>
		''' <param name="to"> </param>
		''' <param name="set"> </param>
		''' <param name="condition"> </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static void replaceWhere(@NonNull INDArray to, @NonNull Number set, @NonNull Condition condition)
		Public Shared Sub replaceWhere(ByVal [to] As INDArray, ByVal set As Number, ByVal condition As Condition)
			If Not (TypeOf condition Is BaseCondition) Then
				Throw New System.NotSupportedException("Only static Conditions are supported")
			End If

			Nd4j.Executioner.exec(New CompareAndSet([to], set.doubleValue(), condition))
		End Sub

		''' <summary>
		''' This method returns first index matching given condition
		''' 
		''' PLEASE NOTE: This method will return -1 value if condition wasn't met
		''' </summary>
		''' <param name="array"> </param>
		''' <param name="condition">
		''' @return </param>
		Public Shared Function firstIndex(ByVal array As INDArray, ByVal condition As Condition) As INDArray
			If Not (TypeOf condition Is BaseCondition) Then
				Throw New System.NotSupportedException("Only static Conditions are supported")
			End If

			Dim idx As New FirstIndex(array, condition)
			Nd4j.Executioner.exec(idx)
			Return Nd4j.scalar(DataType.LONG, idx.FinalResult.longValue())
		End Function

		''' <summary>
		''' This method returns first index matching given condition along given dimensions
		''' 
		''' PLEASE NOTE: This method will return -1 values for missing conditions
		''' </summary>
		''' <param name="array"> </param>
		''' <param name="condition"> </param>
		''' <param name="dimension">
		''' @return </param>
		Public Shared Function firstIndex(ByVal array As INDArray, ByVal condition As Condition, ParamArray ByVal dimension() As Integer) As INDArray
			If Not (TypeOf condition Is BaseCondition) Then
				Throw New System.NotSupportedException("Only static Conditions are supported")
			End If

			Return Nd4j.Executioner.exec(New FirstIndex(array, condition, dimension))
		End Function


		''' <summary>
		''' This method returns last index matching given condition
		''' 
		''' PLEASE NOTE: This method will return -1 value if condition wasn't met
		''' </summary>
		''' <param name="array"> </param>
		''' <param name="condition">
		''' @return </param>
		Public Shared Function lastIndex(ByVal array As INDArray, ByVal condition As Condition) As INDArray
			If Not (TypeOf condition Is BaseCondition) Then
				Throw New System.NotSupportedException("Only static Conditions are supported")
			End If

			Dim idx As New LastIndex(array, condition)
			Nd4j.Executioner.exec(idx)
			Return Nd4j.scalar(DataType.LONG, idx.FinalResult.longValue())
		End Function

		''' <summary>
		''' This method returns first index matching given condition along given dimensions
		''' 
		''' PLEASE NOTE: This method will return -1 values for missing conditions
		''' </summary>
		''' <param name="array"> </param>
		''' <param name="condition"> </param>
		''' <param name="dimension">
		''' @return </param>
		Public Shared Function lastIndex(ByVal array As INDArray, ByVal condition As Condition, ParamArray ByVal dimension() As Integer) As INDArray
			If Not (TypeOf condition Is BaseCondition) Then
				Throw New System.NotSupportedException("Only static Conditions are supported")
			End If

			Return Nd4j.Executioner.exec(New LastIndex(array, condition, dimension))
		End Function
	End Class

End Namespace