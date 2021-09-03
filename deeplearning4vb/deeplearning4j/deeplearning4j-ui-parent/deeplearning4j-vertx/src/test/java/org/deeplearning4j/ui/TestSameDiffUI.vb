Imports System
Imports System.Collections.Generic
Imports System.Threading
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports UIServer = org.deeplearning4j.ui.api.UIServer
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports TempDir = org.junit.jupiter.api.io.TempDir
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports LogFileWriter = org.nd4j.graph.ui.LogFileWriter
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
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

Namespace org.deeplearning4j.ui


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Disabled @Tag(TagNames.FILE_IO) @Tag(TagNames.UI) @Tag(TagNames.DIST_SYSTEMS) @NativeTag public class TestSameDiffUI extends org.deeplearning4j.BaseDL4JTest
	Public Class TestSameDiffUI
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		Private Shared log As Logger = LoggerFactory.getLogger(GetType(TestSameDiffUI).FullName)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Disabled @Test public void testSameDiff(@TempDir Path testDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testSameDiff(ByVal testDir As Path)
			Dim dir As File = testDir.toFile()
			Dim f As New File(dir, "ui_data.bin")
			log.info("File path: {}", f.getAbsolutePath())

			f.getParentFile().mkdirs()
			f.delete()

			Dim sd As SameDiff = SameDiff.create()
			Dim [in] As SDVariable = sd.placeHolder("in", DataType.FLOAT, -1, 3)
			Dim w As SDVariable = sd.var("w", DataType.FLOAT, 3,4)
			Dim b As SDVariable = sd.var("b", DataType.FLOAT, 1, 4)

			Dim z As SDVariable = [in].mmul(w).add(b)
			Dim a As SDVariable = sd.math().tanh(z)

			Dim lfw As New LogFileWriter(f)
			lfw.writeGraphStructure(sd)
			lfw.writeFinishStaticMarker()

			'Append a number of events
			lfw.registerEventName("accuracy")
			lfw.registerEventName("precision")
			Dim t As Long = DateTimeHelper.CurrentUnixTimeMillis()
			For iter As Integer = 0 To 49
				Dim d As Double = Math.Cos(0.1 * iter)
				d *= d
				lfw.writeScalarEvent("accuracy", LogFileWriter.EventSubtype.EVALUATION, t + iter, iter, 0, d)

				Dim prec As Double = Math.Min(0.05 * iter, 1.0)
				lfw.writeScalarEvent("precision", LogFileWriter.EventSubtype.EVALUATION, t+iter, iter, 0, prec)
			Next iter

			'Add some histograms:
			lfw.registerEventName("histogramDiscrete")
			lfw.registerEventName("histogramEqualSpacing")
			lfw.registerEventName("histogramCustomBins")
			For i As Integer = 0 To 2
				Dim discreteY As INDArray = Nd4j.createFromArray(0, 1, 2)
				lfw.writeHistogramEventDiscrete("histogramDiscrete", LogFileWriter.EventSubtype.TUNING_METRIC, t+i, i, 0, New List(Of String) From {"zero", "one", "two"}, discreteY)

				Dim eqSpacingY As INDArray = Nd4j.createFromArray(-0.5 + 0.5 * i, 0.75 * i + i, 1.0 * i + 1.0)
				lfw.writeHistogramEventEqualSpacing("histogramEqualSpacing", LogFileWriter.EventSubtype.TUNING_METRIC, t+i, i, 0, 0.0, 1.0, eqSpacingY)

				Dim customBins As INDArray = Nd4j.createFromArray(New Double()(){
					New Double() {0.0, 0.5, 0.9},
					New Double() {0.2, 0.55, 1.0}
				})
				Console.WriteLine(Arrays.toString(customBins.data().asFloat()))
				Console.WriteLine(customBins.shapeInfoToString())
				lfw.writeHistogramEventCustomBins("histogramCustomBins", LogFileWriter.EventSubtype.TUNING_METRIC, t+i, i, 0, customBins, eqSpacingY)
			Next i


			Dim uiServer As UIServer = UIServer.getInstance()


			Thread.Sleep(1_000_000_000)
		End Sub

	End Class

End Namespace