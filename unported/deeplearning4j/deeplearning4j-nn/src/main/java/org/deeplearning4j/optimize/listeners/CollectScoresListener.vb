Imports System
Imports DoubleArrayList = it.unimi.dsi.fastutil.doubles.DoubleArrayList
Imports IntArrayList = it.unimi.dsi.fastutil.ints.IntArrayList
Imports Data = lombok.Data
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports Model = org.deeplearning4j.nn.api.Model
Imports BaseTrainingListener = org.deeplearning4j.optimize.api.BaseTrainingListener

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

Namespace org.deeplearning4j.optimize.listeners



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data @EqualsAndHashCode(callSuper = true) @Slf4j public class CollectScoresListener extends org.deeplearning4j.optimize.api.BaseTrainingListener implements java.io.Serializable
	<Serializable>
	Public Class CollectScoresListener
		Inherits BaseTrainingListener

		Private ReadOnly frequency As Integer
		Private ReadOnly logScore As Boolean
		Private ReadOnly listIteration As IntArrayList
		Private ReadOnly listScore As DoubleArrayList

		Public Sub New(ByVal frequency As Integer)
			Me.New(frequency, False)
		End Sub

		Public Sub New(ByVal frequency As Integer, ByVal logScore As Boolean)
			Me.frequency = frequency
			Me.logScore = logScore
			listIteration = New IntArrayList()
			listScore = New DoubleArrayList()
		End Sub

		Public Overrides Sub iterationDone(ByVal model As Model, ByVal iteration As Integer, ByVal epoch As Integer)
			If iteration Mod frequency = 0 Then
				Dim score As Double = model.score()
				listIteration.add(iteration)
				listScore.add(score)
				If logScore Then
					log.info("Score at iteration {} is {}", iteration, score)
				End If
			End If
		End Sub
	End Class

End Namespace