Imports lombok
Imports FrameIter = org.nd4j.autodiff.samediff.internal.FrameIter

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

Namespace org.nd4j.autodiff.listeners

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor @EqualsAndHashCode @ToString @Builder @Setter public class At
	Public Class At

'JAVA TO VB CONVERTER NOTE: The field epoch was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private epoch_Conflict As Integer
'JAVA TO VB CONVERTER NOTE: The field iteration was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private iteration_Conflict As Integer
'JAVA TO VB CONVERTER NOTE: The field trainingThreadNum was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private trainingThreadNum_Conflict As Integer
'JAVA TO VB CONVERTER NOTE: The field javaThreadNum was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private javaThreadNum_Conflict As Long
		Private frameIter As FrameIter
'JAVA TO VB CONVERTER NOTE: The field operation was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private operation_Conflict As Operation

		''' <returns> A new instance with everything set to 0, and operation set to INFERENCE </returns>
		Public Shared Function defaultAt() As At
			Return New At(0, 0, 0, 0, Nothing, Operation.INFERENCE)
		End Function

		''' <param name="op"> Operation </param>
		''' <returns> A new instance with everything set to 0, except for the specified operation </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static At defaultAt(@NonNull Operation op)
		Public Shared Function defaultAt(ByVal op As Operation) As At
			Return New At(0, 0, 0, 0, Nothing, op)
		End Function

		''' <returns> The current training epoch </returns>
		Public Overridable Function epoch() As Integer
			Return epoch_Conflict
		End Function

		''' <returns> The current training iteration </returns>
		Public Overridable Function iteration() As Integer
			Return iteration_Conflict
		End Function

		''' <returns> The number of the SameDiff thread </returns>
		Public Overridable Function trainingThreadNum() As Integer
			Return trainingThreadNum_Conflict
		End Function

		''' <returns> The Java/JVM thread number for training </returns>
		Public Overridable Function javaThreadNum() As Long
			Return javaThreadNum_Conflict
		End Function

		''' <returns> The current operation </returns>
		Public Overridable Function operation() As Operation
			Return operation_Conflict
		End Function

		''' <returns> A copy of the current At instance </returns>
		Public Overridable Function copy() As At
			Return New At(epoch_Conflict, iteration_Conflict, trainingThreadNum_Conflict, javaThreadNum_Conflict, frameIter, operation_Conflict)
		End Function

		''' <param name="operation"> Operation to set in the new instance </param>
		''' <returns> A copy of the current instance, but with the specified operation </returns>
		Public Overridable Function copy(ByVal operation As Operation) As At
			Return New At(epoch_Conflict, iteration_Conflict, trainingThreadNum_Conflict, javaThreadNum_Conflict, frameIter, operation)
		End Function
	End Class

End Namespace