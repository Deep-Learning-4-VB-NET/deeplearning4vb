Imports System
Imports System.Collections.Generic
Imports JavaRDD = org.apache.spark.api.java.JavaRDD
Imports BaseSparkTest = org.deeplearning4j.spark.BaseSparkTest
Imports Add = org.deeplearning4j.spark.impl.common.Add
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
import static org.junit.jupiter.api.Assertions.assertEquals

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

Namespace org.deeplearning4j.spark.common


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.FILE_IO) @Tag(TagNames.SPARK) @Tag(TagNames.DIST_SYSTEMS) @NativeTag public class AddTest extends org.deeplearning4j.spark.BaseSparkTest
	<Serializable>
	Public Class AddTest
		Inherits BaseSparkTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testAdd()
		Public Overridable Sub testAdd()
			Dim list As IList(Of INDArray) = New List(Of INDArray)()
			For i As Integer = 0 To 4
				list.Add(Nd4j.ones(5))
			Next i
			Dim rdd As JavaRDD(Of INDArray) = sc.parallelize(list)
			Dim sum As INDArray = rdd.fold(Nd4j.zeros(5), New Add())
			assertEquals(25, sum.sum(Integer.MaxValue).getDouble(0), 1e-1)
		End Sub

	End Class

End Namespace