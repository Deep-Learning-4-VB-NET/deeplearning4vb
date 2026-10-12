Imports System.Collections.Generic
Imports Builder = lombok.Builder
Imports Slf4j = lombok.extern.slf4j.Slf4j
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



	''' <summary>
	''' Using <seealso cref="SystemInfo"/> - it logs a json representation
	''' of system info using slf4j.
	''' 
	''' @author Adam Gibson
	''' </summary>

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @Builder public class SystemInfoPrintListener implements org.deeplearning4j.optimize.api.TrainingListener
	Public Class SystemInfoPrintListener
		Implements TrainingListener

		Private printOnEpochStart As Boolean
		Private printOnEpochEnd As Boolean
		Private printOnForwardPass As Boolean
		Private printOnBackwardPass As Boolean
		Private printOnGradientCalculation As Boolean

		Private Const SYSTEM_INFO As String = "System info on epoch end: "
		Public Overridable Sub iterationDone(ByVal model As Model, ByVal iteration As Integer, ByVal epoch As Integer) Implements TrainingListener.iterationDone

		End Sub

		Public Overridable Sub onEpochStart(ByVal model As Model) Implements TrainingListener.onEpochStart
		   If Not printOnEpochStart Then
			   Return
		   End If

			Dim systemInfo As New SystemInfo()
			log.info("System info on epoch begin: ")
			log.info(systemInfo.toPrettyJSON())
		End Sub

		Public Overridable Sub onEpochEnd(ByVal model As Model) Implements TrainingListener.onEpochEnd
			If Not printOnEpochEnd Then
				Return
			End If

			Dim systemInfo As New SystemInfo()
			log.info(SYSTEM_INFO)
			log.info(systemInfo.toPrettyJSON())
		End Sub

		Public Overridable Sub onForwardPass(ByVal model As Model, ByVal activations As IList(Of INDArray)) Implements TrainingListener.onForwardPass
			If Not printOnBackwardPass Then
				Return
			End If

			Dim systemInfo As New SystemInfo()
			log.info(SYSTEM_INFO)
			log.info(systemInfo.toPrettyJSON())
		End Sub

		Public Overridable Sub onForwardPass(ByVal model As Model, ByVal activations As IDictionary(Of String, INDArray)) Implements TrainingListener.onForwardPass
			If Not printOnForwardPass Then
				Return
			End If

			Dim systemInfo As New SystemInfo()
			log.info(SYSTEM_INFO)
			log.info(systemInfo.toPrettyJSON())
		End Sub

		Public Overridable Sub onGradientCalculation(ByVal model As Model) Implements TrainingListener.onGradientCalculation
			If Not printOnGradientCalculation Then
				Return
			End If

			Dim systemInfo As New SystemInfo()
			log.info(SYSTEM_INFO)
			log.info(systemInfo.toPrettyJSON())
		End Sub

		Public Overridable Sub onBackwardPass(ByVal model As Model) Implements TrainingListener.onBackwardPass
			If Not printOnBackwardPass Then
				Return
			End If
			Dim systemInfo As New SystemInfo()
			log.info(SYSTEM_INFO)
			log.info(systemInfo.toPrettyJSON())
		End Sub
	End Class

End Namespace