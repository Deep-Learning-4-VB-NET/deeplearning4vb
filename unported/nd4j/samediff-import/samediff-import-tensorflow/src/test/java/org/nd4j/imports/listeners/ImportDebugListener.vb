Imports System
Imports NonNull = lombok.NonNull
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports At = org.nd4j.autodiff.listeners.At
Imports BaseListener = org.nd4j.autodiff.listeners.BaseListener
Imports Operation = org.nd4j.autodiff.listeners.Operation
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports SameDiffOp = org.nd4j.autodiff.samediff.internal.SameDiffOp
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports OpContext = org.nd4j.linalg.api.ops.OpContext
Imports MultiDataSet = org.nd4j.linalg.dataset.api.MultiDataSet
Imports Nd4j = org.nd4j.linalg.factory.Nd4j

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

Namespace org.nd4j.imports.listeners


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class ImportDebugListener extends org.nd4j.autodiff.listeners.BaseListener
	Public Class ImportDebugListener
		Inherits BaseListener

		Public Overrides Function isActive(ByVal operation As Operation) As Boolean
			Return True
		End Function

		Public Enum OnFailure
			EXCEPTION
			LOG

		End Enum
		Private baseDir As File
		Private [function] As FilenameFunction
		Private checkShapesOnly As Boolean
		Private fpEps As Double
		Private onFailure As OnFailure
		Private logPass As Boolean

		Public Sub New(ByVal b As Builder)
			Me.baseDir = b.baseDir
			Me.function = b.function
			Me.checkShapesOnly = b.checkShapesOnly_Conflict
			Me.fpEps = b.fpEps
			Me.onFailure = b.onFailure_Conflict
			Me.logPass = b.logPass_Conflict
		End Sub

		Public Overrides Sub opExecution(ByVal sd As SameDiff, ByVal at As At, ByVal batch As MultiDataSet, ByVal op As SameDiffOp, ByVal opContext As OpContext, ByVal outputs() As INDArray)
			'No op

			For i As Integer = 0 To outputs.Length - 1
				Dim f As File = [function].getFileFor(baseDir, op.Name, i)
				If Not f.exists() Then
					log.warn("Skipping check for op {} output {}, no file found: {}", op.Name, i, f.getAbsolutePath())
					Continue For
				End If

				Dim arr As INDArray
				Try
					arr = Nd4j.createFromNpyFile(f)
				Catch t As Exception
					Throw New Exception("Error loading numpy file for op " & op.Name & " - " & f.getAbsolutePath(), t)
				End Try

				If arr.dataType() <> outputs(i).dataType() Then
					Dim msg As String = "Datatype does not match: " & op.Name & ", output " & i & " - TF=" & arr.dataType() & ", SD=" & outputs(i).dataType() & "; TF shape info: " & arr.shapeInfoToString() & " vs. SD shape info: " & outputs(i).shapeInfoToString()
					Select Case onFailure
						Case org.nd4j.imports.listeners.ImportDebugListener.OnFailure.EXCEPTION
							Throw New Exception(msg)
						Case org.nd4j.imports.listeners.ImportDebugListener.OnFailure.LOG
							log.error(msg)
						Case Else
							Throw New Exception()
					End Select
				End If

				If arr.Empty Then
					If Not outputs(i).Empty Then
						Dim msg As String = "TF array is empty but SameDiff output " & i & " is not. TF shape info: " & arr.shapeInfoToString() & " vs. SD shape info: " & outputs(i).shapeInfoToString()
						Select Case onFailure
							Case org.nd4j.imports.listeners.ImportDebugListener.OnFailure.EXCEPTION
								Throw New Exception(msg)
							Case org.nd4j.imports.listeners.ImportDebugListener.OnFailure.LOG
								log.error(msg)
							Case Else
								Throw New Exception()
						End Select
					End If
				End If

				If Not arr.equalShapes(outputs(i)) Then
					Dim msg As String = "SameDiff output " & i & " does not match TF shape. TF shape info: " & arr.shapeInfoToString() & " vs. SD shape info: " & outputs(i).shapeInfoToString()
					Select Case onFailure
						Case org.nd4j.imports.listeners.ImportDebugListener.OnFailure.EXCEPTION
							Throw New Exception(msg)
						Case org.nd4j.imports.listeners.ImportDebugListener.OnFailure.LOG
							log.error(msg)
						Case Else
							Throw New Exception()
					End Select
				End If

				If checkShapesOnly Then
					Continue For
				End If

				Dim eq As Boolean = arr.equalsWithEps(outputs(i), fpEps)
				If Not eq Then
					Dim msg As String = "SameDiff output " & i & " does not match TF values with eps=" & fpEps & ". TF shape info: " & arr.shapeInfoToString() & " vs. SD shape info: " & outputs(i).shapeInfoToString()
					Select Case onFailure
						Case org.nd4j.imports.listeners.ImportDebugListener.OnFailure.EXCEPTION
							Throw New Exception(msg)
						Case org.nd4j.imports.listeners.ImportDebugListener.OnFailure.LOG
							log.error(msg)
						Case Else
							Throw New Exception()
					End Select
				End If

				If logPass Then
					log.info("Passed: {} output {}", op.Name, i)
				End If
			Next i
		End Sub

		Public Shared Function builder(ByVal rootDir As File) As Builder
			Return New Builder(rootDir)
		End Function


		Public Class Builder

			Friend baseDir As File
			Friend [function] As FilenameFunction = New DefaultFilenameFunction()
'JAVA TO VB CONVERTER NOTE: The field checkShapesOnly was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend checkShapesOnly_Conflict As Boolean = False
			Friend fpEps As Double = 1e-5
'JAVA TO VB CONVERTER NOTE: The field onFailure was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend onFailure_Conflict As OnFailure = OnFailure.EXCEPTION
'JAVA TO VB CONVERTER NOTE: The field logPass was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend logPass_Conflict As Boolean = False


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder(@NonNull File baseDir)
			Public Sub New(ByVal baseDir As File)
				Me.baseDir = baseDir
			End Sub

'JAVA TO VB CONVERTER NOTE: The parameter onFailure was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function onFailure(ByVal onFailure_Conflict As OnFailure) As Builder
				Me.onFailure_Conflict = onFailure_Conflict
				Return Me
			End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder filenameFunction(@NonNull FilenameFunction fn)
			Public Overridable Function filenameFunction(ByVal fn As FilenameFunction) As Builder
				Me.function = fn
				Return Me
			End Function

			Public Overridable Function checkShapesOnly(ByVal shapesOnly As Boolean) As Builder
				Me.checkShapesOnly_Conflict = shapesOnly
				Return Me
			End Function

			Public Overridable Function floatingPointEps(ByVal eps As Double) As Builder
				Me.fpEps = eps
				Return Me
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter logPass was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function logPass(ByVal logPass_Conflict As Boolean) As Builder
				Me.logPass_Conflict = logPass_Conflict
				Return Me
			End Function

			Public Overridable Function build() As ImportDebugListener
				Return New ImportDebugListener(Me)
			End Function

		End Class

		Public Interface FilenameFunction
			Function getFileFor(ByVal rootDir As File, ByVal opName As String, ByVal outputNum As Integer) As File

		End Interface

		Public Class DefaultFilenameFunction
			Implements FilenameFunction

			Public Overridable Function getFileFor(ByVal rootDir As File, ByVal opName As String, ByVal outputNum As Integer) As File Implements FilenameFunction.getFileFor
				Return New File(rootDir, opName & "__" & outputNum & ".npy")
			End Function
		End Class
	End Class

End Namespace