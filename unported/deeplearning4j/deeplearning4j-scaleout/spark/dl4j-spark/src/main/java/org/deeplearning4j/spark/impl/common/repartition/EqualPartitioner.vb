Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports Partitioner = org.apache.spark.Partitioner
Imports JavaRDD = org.apache.spark.api.java.JavaRDD

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

Namespace org.deeplearning4j.spark.impl.common.repartition


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @AllArgsConstructor public class EqualPartitioner extends org.apache.spark.Partitioner
	Public Class EqualPartitioner
		Inherits Partitioner

'JAVA TO VB CONVERTER NOTE: The field numPartitions was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private ReadOnly numPartitions_Conflict As Integer 'Total number of partitions
		Private ReadOnly partitionSizeExRemainder As Integer
		Private ReadOnly remainderPositions() As Integer

		Public Overrides Function numPartitions() As Integer
			Return numPartitions_Conflict
		End Function

		Public Overrides Function getPartition(ByVal key As Object) As Integer
			Dim elementIdx As Integer = key.GetHashCode()

			'Assign an equal number of elements to each partition, sequentially
			' For any remainder, use the specified remainder indexes

			'Work out: which partition it belongs to...
			If elementIdx < numPartitions_Conflict * partitionSizeExRemainder Then
				'Standard element
				Return elementIdx \ partitionSizeExRemainder
			Else
				'Is a 'remainder' element
				Dim remainderNum As Integer = elementIdx Mod numPartitions_Conflict
				Return remainderPositions(remainderNum Mod remainderPositions.Length) 'Final mod here shouldn't be necessary, but is here for safety...
			End If
		End Function
	End Class

End Namespace