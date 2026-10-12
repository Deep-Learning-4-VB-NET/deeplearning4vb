Imports System.Collections.Generic
Imports NonNull = lombok.NonNull
Imports Builder = lombok.Builder
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports FileUtils = org.apache.commons.io.FileUtils
Imports Model = org.deeplearning4j.nn.api.Model
Imports TrainingListener = org.deeplearning4j.optimize.api.TrainingListener
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports SystemInfo = oshi.json.SystemInfo

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

Namespace org.deeplearning4j.core.listener


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @Builder public class SystemInfoFilePrintListener implements org.deeplearning4j.optimize.api.TrainingListener
	Public Class SystemInfoFilePrintListener
		Implements TrainingListener

		Private printOnEpochStart As Boolean
		Private printOnEpochEnd As Boolean
		Private printOnForwardPass As Boolean
		Private printOnBackwardPass As Boolean
		Private printOnGradientCalculation As Boolean
		Private printFileTarget As File

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public SystemInfoFilePrintListener(boolean printOnEpochStart, boolean printOnEpochEnd, boolean printOnForwardPass, boolean printOnBackwardPass, boolean printOnGradientCalculation, @NonNull File printFileTarget)
		Public Sub New(ByVal printOnEpochStart As Boolean, ByVal printOnEpochEnd As Boolean, ByVal printOnForwardPass As Boolean, ByVal printOnBackwardPass As Boolean, ByVal printOnGradientCalculation As Boolean, ByVal printFileTarget As File)
			Me.printOnEpochStart = printOnEpochStart
			Me.printOnEpochEnd = printOnEpochEnd
			Me.printOnForwardPass = printOnForwardPass
			Me.printOnBackwardPass = printOnBackwardPass
			Me.printOnGradientCalculation = printOnGradientCalculation
			Me.printFileTarget = printFileTarget

		End Sub

		Public Overridable Sub iterationDone(ByVal model As Model, ByVal iteration As Integer, ByVal epoch As Integer) Implements TrainingListener.iterationDone

		End Sub

		Public Overridable Sub onEpochStart(ByVal model As Model) Implements TrainingListener.onEpochStart
			If Not printOnEpochStart OrElse printFileTarget Is Nothing Then
				Return
			End If

			writeFileWithMessage("epoch end")

		End Sub

		Public Overridable Sub onEpochEnd(ByVal model As Model) Implements TrainingListener.onEpochEnd
			If Not printOnEpochEnd OrElse printFileTarget Is Nothing Then
				Return
			End If

			writeFileWithMessage("epoch begin")

		End Sub

		Public Overridable Sub onForwardPass(ByVal model As Model, ByVal activations As IList(Of INDArray)) Implements TrainingListener.onForwardPass
			If Not printOnBackwardPass OrElse printFileTarget Is Nothing Then
				Return
			End If

			writeFileWithMessage("forward pass")

		End Sub

		Public Overridable Sub onForwardPass(ByVal model As Model, ByVal activations As IDictionary(Of String, INDArray)) Implements TrainingListener.onForwardPass
			If Not printOnForwardPass OrElse printFileTarget Is Nothing Then
				Return
			End If

			writeFileWithMessage("forward pass")

		End Sub

		Public Overridable Sub onGradientCalculation(ByVal model As Model) Implements TrainingListener.onGradientCalculation
			If Not printOnGradientCalculation OrElse printFileTarget Is Nothing Then
				Return
			End If

			writeFileWithMessage("gradient calculation")


		End Sub

		Public Overridable Sub onBackwardPass(ByVal model As Model) Implements TrainingListener.onBackwardPass
			If Not printOnBackwardPass OrElse printFileTarget Is Nothing Then
				Return
			End If

			writeFileWithMessage("backward pass")
		End Sub

		Private Sub writeFileWithMessage(ByVal status As String)
			If printFileTarget Is Nothing Then
				log.warn("File not specified for writing!")
			End If

			Dim systemInfo As New SystemInfo()
			log.info("Writing system info to file on " & status & ": " & printFileTarget.getAbsolutePath())
			Try
				FileUtils.write(printFileTarget,systemInfo.toPrettyJSON(), True)
			Catch e As IOException
				log.error("Error writing file for system info",e)
			End Try
		End Sub
	End Class



End Namespace