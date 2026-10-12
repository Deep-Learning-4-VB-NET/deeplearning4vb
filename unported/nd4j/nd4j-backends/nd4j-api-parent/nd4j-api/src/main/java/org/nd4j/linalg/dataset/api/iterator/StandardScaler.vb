Imports System
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Transforms = org.nd4j.linalg.ops.transforms.Transforms
Imports Logger = org.slf4j.Logger
Imports LoggerFactory = org.slf4j.LoggerFactory

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

Namespace org.nd4j.linalg.dataset.api.iterator



	<Obsolete>
	Public Class StandardScaler
		Private Shared logger As Logger = LoggerFactory.getLogger(GetType(StandardScaler))
'JAVA TO VB CONVERTER NOTE: The field mean was renamed since Visual Basic does not allow fields to have the same name as other class members:
'JAVA TO VB CONVERTER NOTE: The field std was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private mean_Conflict, std_Conflict As INDArray
		Private runningTotal As Long = 0
		Private batchCount As Long = 0

		Public Overridable Sub fit(ByVal dataSet As DataSet)
			mean_Conflict = dataSet.Features.mean(0)
			std_Conflict = dataSet.Features.std(0)
			std_Conflict.addi(Nd4j.scalar(Nd4j.EPS_THRESHOLD))
			If std_Conflict.min(1) Is Nd4j.scalar(Nd4j.EPS_THRESHOLD) Then
				logger.info("API_INFO: Std deviation found to be zero. Transform will round upto epsilon to avoid nans.")
			End If
		End Sub

		''' <summary>
		''' Fit the given model </summary>
		''' <param name="iterator"> the data to iterate oer </param>
		Public Overridable Sub fit(ByVal iterator As DataSetIterator)
			Do While iterator.MoveNext()
				Dim [next] As DataSet = iterator.Current
				runningTotal += [next].numExamples()
				batchCount = [next].Features.size(0)
				If mean_Conflict Is Nothing Then
					'start with the mean and std of zero
					'column wise
					mean_Conflict = [next].Features.mean(0)
					std_Conflict = If(batchCount = 1, Nd4j.zeros(mean_Conflict.shape()), Transforms.pow([next].Features.std(0), 2))
					std_Conflict.muli(batchCount)
				Else
					' m_newM = m_oldM + (x - m_oldM)/m_n;
					' This only works if batch size is 1, m_newS = m_oldS + (x - m_oldM)*(x - m_newM);
					Dim xMinusMean As INDArray = [next].Features.subRowVector(mean_Conflict)
					Dim newMean As INDArray = mean_Conflict.add(xMinusMean.sum(0).divi(runningTotal))
					' Using http://i.stanford.edu/pub/cstr/reports/cs/tr/79/773/CS-TR-79-773.pdf
					' for a version of calc variance when dataset is partitioned into two sample sets
					' Also described in https://en.wikipedia.org/wiki/Algorithms_for_calculating_variance#Parallel_algorithm
					' delta = mean_B - mean_A; A is data seen so far, B is the current batch
					' M2 is the var*n
					' M2 = M2_A + M2_B + delta^2 * nA * nB/(nA+nB)
					Dim meanB As INDArray = [next].Features.mean(0)
					Dim deltaSq As INDArray = Transforms.pow(meanB.subRowVector(mean_Conflict), 2)
					Dim deltaSqScaled As INDArray = deltaSq.mul((CSng(runningTotal) - batchCount) * batchCount / CSng(runningTotal))
					Dim mtwoB As INDArray = Transforms.pow([next].Features.std(0), 2)
					mtwoB.muli(batchCount)
					std_Conflict = std_Conflict.add(mtwoB)
					std_Conflict = std_Conflict.add(deltaSqScaled)
					mean_Conflict = newMean
				End If

			Loop
			std_Conflict.divi(runningTotal)
			std_Conflict = Transforms.sqrt(std_Conflict)
			std_Conflict.addi(Nd4j.scalar(Nd4j.EPS_THRESHOLD))
			If std_Conflict.min(1) Is Nd4j.scalar(Nd4j.EPS_THRESHOLD) Then
				logger.info("API_INFO: Std deviation found to be zero. Transform will round upto epsilon to avoid nans.")
			End If
			iterator.reset()
		End Sub


		''' <summary>
		''' Load the given mean and std </summary>
		''' <param name="mean"> the mean file </param>
		''' <param name="std"> the std file </param>
		''' <exception cref="IOException"> </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public void load(java.io.File mean, java.io.File std) throws java.io.IOException
		Public Overridable Sub load(ByVal mean As File, ByVal std As File)
			Me.mean_Conflict = Nd4j.readBinary(mean)
			Me.std_Conflict = Nd4j.readBinary(std)
		End Sub

		''' <summary>
		''' Save the current mean and std </summary>
		''' <param name="mean"> the mean </param>
		''' <param name="std"> the std </param>
		''' <exception cref="IOException"> </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public void save(java.io.File mean, java.io.File std) throws java.io.IOException
		Public Overridable Sub save(ByVal mean As File, ByVal std As File)
			Nd4j.saveBinary(Me.mean_Conflict, mean)
			Nd4j.saveBinary(Me.std_Conflict, std)
		End Sub

		''' <summary>
		''' Transform the data </summary>
		''' <param name="dataSet"> the dataset to transform </param>
		Public Overridable Sub transform(ByVal dataSet As DataSet)
			dataSet.Features = dataSet.Features.subRowVector(mean_Conflict)
			dataSet.Features = dataSet.Features.divRowVector(std_Conflict)
		End Sub


		Public Overridable ReadOnly Property Mean As INDArray
			Get
				Return mean_Conflict
			End Get
		End Property

		Public Overridable ReadOnly Property Std As INDArray
			Get
				Return std_Conflict
			End Get
		End Property
	End Class

End Namespace