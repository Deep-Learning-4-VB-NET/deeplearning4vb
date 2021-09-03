Imports System
Imports NonNull = lombok.NonNull
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports org.deeplearning4j.models.sequencevectors.sequence
Imports SentenceTransformer = org.deeplearning4j.models.sequencevectors.transformers.impl.SentenceTransformer
Imports VocabWord = org.deeplearning4j.models.word2vec.VocabWord
Imports AsyncLabelAwareIterator = org.deeplearning4j.text.documentiterator.AsyncLabelAwareIterator
Imports LabelAwareIterator = org.deeplearning4j.text.documentiterator.LabelAwareIterator
Imports LabelledDocument = org.deeplearning4j.text.documentiterator.LabelledDocument

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

Namespace org.deeplearning4j.models.sequencevectors.transformers.impl.iterables


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class ParallelTransformerIterator extends BasicTransformerIterator
	Public Class ParallelTransformerIterator
		Inherits BasicTransformerIterator

		Protected Friend Const capacity As Integer = 1024
		Protected Friend buffer As BlockingQueue(Of Future(Of Sequence(Of VocabWord))) = New LinkedBlockingQueue(Of Future(Of Sequence(Of VocabWord)))(capacity)
		'protected BlockingQueue<LabelledDocument> stringBuffer;
		'protected TokenizerThread[] threads;
		Protected Friend underlyingHas As New AtomicBoolean(True)
		Protected Friend processing As New AtomicInteger(0)

		Private executorService As ExecutorService

		Protected Friend Shared ReadOnly count As New AtomicInteger(0)

		Private Const PREFETCH_SIZE As Integer = 100

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public ParallelTransformerIterator(@NonNull LabelAwareIterator iterator, @NonNull SentenceTransformer transformer)
		Public Sub New(ByVal iterator As LabelAwareIterator, ByVal transformer As SentenceTransformer)
			Me.New(iterator, transformer, True)
		End Sub

		Private Sub prefetchIterator()
	'        for (int i = 0; i < PREFETCH_SIZE; ++i) {
	'            //boolean before = underlyingHas;
	'
	'                if (underlyingHas.get())
	'                    underlyingHas.set(iterator.hasNextDocument());
	'                else
	'                    underlyingHas.set(false);
	'
	'            if (underlyingHas.get()) {
	'                CallableTransformer callableTransformer = new CallableTransformer(iterator.nextDocument(), sentenceTransformer);
	'                Future<Sequence<VocabWord>> futureSequence = executorService.submit(callableTransformer);
	'                try {
	'                    buffer.put(futureSequence);
	'                } catch (InterruptedException e) {
	'                    e.printStackTrace();
	'                }
	'            }
	'        }
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public ParallelTransformerIterator(@NonNull LabelAwareIterator iterator, @NonNull SentenceTransformer transformer, boolean allowMultithreading)
		Public Sub New(ByVal iterator As LabelAwareIterator, ByVal transformer As SentenceTransformer, ByVal allowMultithreading As Boolean)
			MyBase.New(New AsyncLabelAwareIterator(iterator, 512), transformer)
			'super(iterator, transformer);
			Me.allowMultithreading = allowMultithreading
			'this.stringBuffer = new LinkedBlockingQueue<>(512);

			'threads = new TokenizerThread[1];
			'threads = new TokenizerThread[allowMultithreading ? Math.max(Runtime.getRuntime().availableProcessors(), 2) : 1];
			executorService = Executors.newFixedThreadPool(If(allowMultithreading, Math.Max(Runtime.getRuntime().availableProcessors(), 2), 1))

			prefetchIterator()
		End Sub

		Public Overrides Sub reset()
			Me.executorService.shutdownNow()
			Me.iterator.reset()
			underlyingHas.set(True)
			prefetchIterator()
			Me.buffer.clear()
			Me.executorService = Executors.newFixedThreadPool(If(allowMultithreading, Math.Max(Runtime.getRuntime().availableProcessors(), 2), 1))
		End Sub

		Public Overridable Sub shutdown()
			executorService.shutdown()
		End Sub

		Private Class CallableTransformer
			Implements Callable(Of Sequence(Of VocabWord))

			Friend document As LabelledDocument
			Friend transformer As SentenceTransformer

			Public Sub New(ByVal document As LabelledDocument, ByVal transformer As SentenceTransformer)
				Me.transformer = transformer
				Me.document = document
			End Sub

			Public Overrides Function [call]() As Sequence(Of VocabWord)
				Dim sequence As New Sequence(Of VocabWord)()

				If document IsNot Nothing AndAlso document.getContent() IsNot Nothing Then
					sequence = transformer.transformToSequence(document.getContent())
					If document.getLabels() IsNot Nothing Then
						For Each label As String In document.getLabels()
							If label IsNot Nothing AndAlso label.Length > 0 Then
								sequence.addSequenceLabel(New VocabWord(1.0, label))
							End If
						Next label
					End If
				End If
				Return sequence
			End Function
		End Class

		Public Overrides Function hasNext() As Boolean
			'boolean before = underlyingHas;

			'if (underlyingHas.get()) {
				If buffer.size() < capacity AndAlso iterator.hasNextDocument() Then
					Dim transformer As New CallableTransformer(iterator.nextDocument(), sentenceTransformer)
					Dim futureSequence As Future(Of Sequence(Of VocabWord)) = executorService.submit(transformer)
					Try
						buffer.put(futureSequence)
					Catch e As InterruptedException
						log.error("",e)
					End Try
				End If
	'            else
	'                underlyingHas.set(false);
	'
	'        }
	'        else {
	'           underlyingHas.set(false);
	'        }

			Return (Not buffer.isEmpty() OrElse processing.get() > 0)
		End Function

		Public Overrides Function [next]() As Sequence(Of VocabWord)
			Try
	'            if (underlyingHas)
	'                stringBuffer.put(iterator.nextDocument());
				processing.incrementAndGet()
				Dim future As Future(Of Sequence(Of VocabWord)) = buffer.take()
				Dim sequence As Sequence(Of VocabWord) = future.get()
				processing.decrementAndGet()
				Return sequence

			Catch e As Exception
				Throw New Exception(e)
			End Try
		End Function
	End Class

End Namespace