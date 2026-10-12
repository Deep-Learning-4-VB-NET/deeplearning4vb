Imports System.Collections.Generic
Imports Getter = lombok.Getter
Imports NonNull = lombok.NonNull
Imports org.deeplearning4j.rl4j.agent.learning.algorithm
Imports org.deeplearning4j.rl4j.agent.learning.update.updater

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
Namespace org.deeplearning4j.rl4j.agent.learning.update


	Public Class UpdateRule(Of RESULT_TYPE, EXPERIENCE_TYPE)
		Implements IUpdateRule(Of EXPERIENCE_TYPE)

		Private ReadOnly updater As INeuralNetUpdater

		Private ReadOnly updateAlgorithm As IUpdateAlgorithm(Of RESULT_TYPE, EXPERIENCE_TYPE)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private int updateCount = 0;
		Private updateCount As Integer = 0

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public UpdateRule(@NonNull IUpdateAlgorithm<RESULT_TYPE, EXPERIENCE_TYPE> updateAlgorithm, @NonNull INeuralNetUpdater<RESULT_TYPE> updater)
		Public Sub New(ByVal updateAlgorithm As IUpdateAlgorithm(Of RESULT_TYPE, EXPERIENCE_TYPE), ByVal updater As INeuralNetUpdater(Of RESULT_TYPE))
			Me.updateAlgorithm = updateAlgorithm
			Me.updater = updater
		End Sub

		Public Overridable Sub update(ByVal trainingBatch As IList(Of EXPERIENCE_TYPE)) Implements IUpdateRule(Of EXPERIENCE_TYPE).update
			Dim featuresLabels As RESULT_TYPE = updateAlgorithm.compute(trainingBatch)
			updater.update(featuresLabels)
			updateCount += 1
		End Sub

		Public Overridable Sub notifyNewBatchStarted() Implements IUpdateRule(Of EXPERIENCE_TYPE).notifyNewBatchStarted
			updater.synchronizeCurrent()
		End Sub

	End Class

End Namespace