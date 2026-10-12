Imports AffinityManager = org.nd4j.linalg.api.concurrency.AffinityManager
Imports DataSet = org.nd4j.linalg.dataset.api.DataSet
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

Namespace org.nd4j.linalg.dataset.callbacks

	Public Class DefaultCallback
		Implements DataSetCallback

		Public Overridable Sub [call](ByVal dataSet As DataSet) Implements DataSetCallback.call
			If dataSet IsNot Nothing Then
				If dataSet.Features IsNot Nothing Then
					Nd4j.AffinityManager.ensureLocation(dataSet.Features, AffinityManager.Location.DEVICE)
				End If

				If dataSet.Labels IsNot Nothing Then
					Nd4j.AffinityManager.ensureLocation(dataSet.Labels, AffinityManager.Location.DEVICE)
				End If

				If dataSet.FeaturesMaskArray IsNot Nothing Then
					Nd4j.AffinityManager.ensureLocation(dataSet.FeaturesMaskArray, AffinityManager.Location.DEVICE)
				End If

				If dataSet.LabelsMaskArray IsNot Nothing Then
					Nd4j.AffinityManager.ensureLocation(dataSet.LabelsMaskArray, AffinityManager.Location.DEVICE)
				End If
			End If
		End Sub

		Public Overridable Sub [call](ByVal multiDataSet As MultiDataSet) Implements DataSetCallback.call
			If multiDataSet IsNot Nothing Then
				If multiDataSet.Features IsNot Nothing Then
					Dim i As Integer = 0
					Do While i < multiDataSet.Features.Length
						Nd4j.AffinityManager.ensureLocation(multiDataSet.Features(i), AffinityManager.Location.DEVICE)
						i += 1
					Loop
				End If

				If multiDataSet.Labels IsNot Nothing Then
					Dim i As Integer = 0
					Do While i < multiDataSet.Labels.Length
						Nd4j.AffinityManager.ensureLocation(multiDataSet.Labels(i), AffinityManager.Location.DEVICE)
						i += 1
					Loop
				End If

				If multiDataSet.FeaturesMaskArrays IsNot Nothing Then
					Dim i As Integer = 0
					Do While i < multiDataSet.FeaturesMaskArrays.Length
						Nd4j.AffinityManager.ensureLocation(multiDataSet.FeaturesMaskArrays(i), AffinityManager.Location.DEVICE)
						i += 1
					Loop
				End If

				If multiDataSet.LabelsMaskArrays IsNot Nothing Then
					Dim i As Integer = 0
					Do While i < multiDataSet.LabelsMaskArrays.Length
						Nd4j.AffinityManager.ensureLocation(multiDataSet.LabelsMaskArrays(i), AffinityManager.Location.DEVICE)
						i += 1
					Loop
				End If
			End If
		End Sub

		Public Overridable Sub reset() Implements DataSetCallback.reset
			' do nothing
		End Sub
	End Class

End Namespace